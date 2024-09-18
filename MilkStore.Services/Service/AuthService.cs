using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MilkStore.ModelViews.AuthModelViews;
using MilkStore.Repositories.Entity;
using Microsoft.AspNetCore.Identity;
using MilkStore.Core.Base;
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;

    public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;

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
}