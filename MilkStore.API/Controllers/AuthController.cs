using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core.Base;
using MilkStore.ModelViews.AuthModelViews;
using MilkStore.Repositories.Entity;

namespace MilkStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly IUserService userService;
        private readonly SignInManager<ApplicationUser> signInManager;
        public AuthController(IAuthService authService, IUserService userService, SignInManager<ApplicationUser> signInManager)
        {
            this.authService = authService;
            this.userService = userService;
            this.signInManager = signInManager;
        }

        [HttpPost("auth_account")]
        public async Task<IActionResult> Login(LoginModelView model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseException.BadRequestException("BadRequest", ModelState.ToString()));
            }
            else
            {
                try
                {
                    ApplicationUser result = await authService.CheckUser(model.Username);
                    Microsoft.AspNetCore.Identity.SignInResult resultPassword = await authService.CheckPassword(model);
                    if (!resultPassword.Succeeded)
                    {
                        return Unauthorized(new BaseException.ErrorException(401, "Unauthorized", "Không đúng mật khẩu"));
                    }
                    (string token, IEnumerable<string> roles) = authService.GenerateJwtToken(result);
                    return Ok(BaseResponse<object>.OkResponse(new
                    {
                        access_token = token,
                        token_type = "JWT",
                        auth_type = "Bearer",
                        expires_in = DateTime.UtcNow.AddHours(1),
                        user = new
                        {
                            userName = result.UserName,
                            email = result.Email,
                            role = roles
                        }
                    }));

                }
                catch (BaseException.ErrorException e)
                {

                    return StatusCode(e.StatusCode, new { message = e.ErrorDetail.ErrorMessage, code = e.ErrorDetail.ErrorCode });
                }
            }
        }

        [HttpPost("new_account")]
        public async Task<IActionResult> Register(RegisterModelView model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseException.BadRequestException("BadRequest", ModelState.ToString()));
            }
            ApplicationUser existingEmail = await userService.GetUserByEmail(model.Email);
            if (existingEmail != null)
            {
                return BadRequest(new BaseException.ErrorException(400, "BadRequest", "Email đã tồn tại!"));
            }
            ApplicationUser existingUserName = await authService.CheckUser(model.Username);
            if (existingUserName != null)
            {
                return BadRequest(new BaseException.ErrorException(400, "BadRequest", "Tên đăng nhập đã tồn tại!"));
            }
            IdentityResult result = await userService.CreateUser(model);
            if (result.Succeeded)
            {
                return Ok(BaseResponse<string>.OkResponse("Tạo thành công"));
            }
            return StatusCode(500, new BaseException.ErrorException(500, "InternalServerError", "Lỗi khi tạo người dùng"));
        }
        [HttpGet("signin-google")]
        public IActionResult LoginGoogle()
        {
            AuthenticationProperties properties = new()
            {
                RedirectUri = Url.Action(nameof(GoogleResponse))
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }
        [HttpGet("signin-google/callback")]
        public async Task<IActionResult> GoogleResponse()
        {
            AuthenticateResult result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                return Unauthorized(new BaseException.ErrorException(401, "Unauthorized", "Xác thực không thành công"));
            }
            IEnumerable<Claim> claims = result.Principal.Claims;

            LoginGoogleModel loginModel = new()
            {
                Gmail = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                ProviderKey = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
            };
            ApplicationUser existingEmail = await userService.GetUserByEmail(loginModel.Gmail);
            if (existingEmail != null)
            {
                if (existingEmail.DeletedTime.HasValue)
                {
                    return BadRequest(new BaseException.ErrorException(400, "BadRequest", "Tài khoản đã bị xóa"));
                }
                (string token, IEnumerable<string> roles) = authService.GenerateJwtToken(existingEmail);
                return Ok(BaseResponse<object>.OkResponse(new
                {
                    access_token = token,
                    token_type = "JWT",
                    auth_type = "Bearer",
                    expires_in = DateTime.UtcNow.AddHours(1),
                    user = new
                    {
                        email = existingEmail.Email,
                        role = roles
                    }
                }));

            }
            else
            {
                IdentityResult createUserGoogle = await userService.CreateUserLoginGoogle(loginModel);
                if (createUserGoogle.Succeeded)
                {
                    (string token, IEnumerable<string> roles) = authService.GenerateJwtToken(existingEmail);
                    return Ok(BaseResponse<object>.OkResponse(new
                    {
                        access_token = token,
                        token_type = "JWT",
                        auth_type = "Bearer",
                        expires_in = DateTime.UtcNow.AddHours(1),
                        user = new
                        {
                            email = existingEmail.Email,
                            role = roles
                        }
                    }));
                }
                else
                {
                    return StatusCode(500, new BaseException.ErrorException(500, "InternalServerError", $"Lỗi khi tạo người dùng {createUserGoogle.Errors.FirstOrDefault()?.Description}"));
                }
            }


        }
    }
}
