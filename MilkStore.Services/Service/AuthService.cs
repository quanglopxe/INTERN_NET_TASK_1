using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MilkStore.ModelViews.AuthModelViews;
using MilkStore.Repositories.Entity;
using Microsoft.AspNetCore.Identity;
using MilkStore.Core.Base;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Repositories.UOW;
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly UnitOfWork unitOfWork;

    public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, UnitOfWork unitOfWork)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.unitOfWork = unitOfWork;

    }
    public async Task<ApplicationUser> ExistingUser(string email)
    {
        ApplicationUser? user = await userManager.FindByEmailAsync(email);
        if (user != null)
        {
            if (user.DeletedTime.HasValue)
            {
                throw new BaseException.ErrorException(400, "BadRequest", "Tài khoản đã bị xóa");
            }
            else
            {
                return user;
            }
        }
        else
        {
            throw new BaseException.ErrorException(404, "NotFound", "Không tìm thấy người dùng");
        }
    }
    public async Task<SignInResult> CheckPassword(LoginModelView loginModel)
    {
        SignInResult result = await signInManager.PasswordSignInAsync(loginModel.Email, loginModel.Password, false, false);
        if (!result.Succeeded)
        {
            throw new BaseException.ErrorException(401, "Unauthorized", "Không đúng mật khẩu");
        }
        return result;
    }

    public async Task ChangePasswordAdmin(string id, ChangePasswordAdminModel model)
    {
        ApplicationUser? admin = await userManager.FindByIdAsync(id);
        if (admin is null)
        {
            throw new BaseException.ErrorException(404, "NotFound", "Không tìm thấy người dùng");
        }
        else
        {
            if (admin.DeletedTime.HasValue)
            {
                throw new BaseException.ErrorException(400, "BadRequest", "Tài khoản đã bị xóa");
            }
            else
            {
                IdentityResult result = await userManager.ChangePasswordAsync(admin, model.OldPassword, model.NewPassword);
                if (!result.Succeeded)
                {
                    throw new BaseException.ErrorException(500, " InternalServerError", result.Errors.FirstOrDefault()?.Description);
                }
            }
        }
    }


    public (string token, IEnumerable<string> roles) GenerateJwtToken(ApplicationUser user)
    {
        byte[] key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY") ?? throw new Exception("JWT_KEY is not set"));
        List<Claim> claims = new List<Claim> {
            new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };
        IEnumerable<string> roles = userManager.GetRolesAsync(user: user).Result;
        foreach (string role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? throw new Exception("JWT_ISSUER is not set"),
            Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? throw new Exception("JWT_AUDIENCE is not set"),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        return (tokenHandler.WriteToken(token), roles);
    }
    public async Task<string> GenerateRefreshToken(ApplicationUser user)
    {
        string? refreshToken = Guid.NewGuid().ToString();
        DateTime expiryDate = DateTime.UtcNow.AddDays(30);
        ApplicationUserTokens? userToken = new ApplicationUserTokens
        {
            UserId = user.Id,
            LoginProvider = "Default",
            Name = "RefreshToken",
            Value = refreshToken,

        };

        unitOfWork.GetRepository<ApplicationUserTokens>().Insert(userToken);
        await unitOfWork.SaveAsync();
        return Guid.NewGuid().ToString();
    }
}