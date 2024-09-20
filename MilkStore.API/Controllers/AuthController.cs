using System.Security.Claims;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
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
                    ApplicationUser result = await authService.ExistingUser(model.Email);
                    Microsoft.AspNetCore.Identity.SignInResult resultPassword = await authService.CheckPassword(model);
                    (string token, IEnumerable<string> roles) = authService.GenerateJwtToken(result);
                    string refreshToken = await authService.GenerateRefreshToken(result);
                    return Ok(BaseResponse<object>.OkResponse(new
                    {
                        access_token = token,
                        refreshToke = refreshToken,
                        token_type = "JWT",
                        auth_type = "Bearer",
                        expires_in = DateTime.UtcNow.AddHours(1),
                        user = new
                        {
                            email = result.Email,
                            role = roles
                        }
                    }));

                }
                catch (BaseException.ErrorException e)
                {

                    return StatusCode(e.StatusCode, new BaseException.ErrorException(e.StatusCode, e.ErrorDetail.ErrorCode, e.ErrorDetail.ErrorMessage.ToString()));
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
            else
            {
                try
                {
                    await userService.GetUserByEmailToRegister(model.Email);
                    await userService.CreateUser(model);
                    return Ok(BaseResponse<string>.OkResponse("Tạo thành công"));
                }
                catch (BaseException.ErrorException e)
                {

                    return StatusCode(e.StatusCode, new BaseException.ErrorException(e.StatusCode, e.ErrorDetail.ErrorCode, e.ErrorDetail.ErrorMessage.ToString()));
                }
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPatch("Change_Password_Admin")]
        public async Task<IActionResult> ChangePasswordForAdmin(ChangePasswordAdminModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseException.ErrorException(400, "BadRequest", ModelState.ToString()));
            }
            else
            {
                try
                {
                    // get id from token
                    string? Id = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    await authService.ChangePasswordAdmin(Id, model);
                    return Ok(BaseResponse<string>.OkResponse("Đổi mật khẩu thành công"));
                }
                catch (BaseException.ErrorException ex)
                {
                    return StatusCode(ex.StatusCode, new BaseException.ErrorException(ex.StatusCode, ex.ErrorDetail.ErrorCode, ex.ErrorDetail.ErrorMessage.ToString()));

                }
            }
        }
        [HttpPost("signin-google")]
        public async Task<IActionResult> LoginGoogle([FromBody] TokenGoogleModel tokenGoogle)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseException.ErrorException(400, "BadRequest", ModelState.ToString()));
            }
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(tokenGoogle.token);
                string email = payload.Email;
                string providerKey = payload.Subject;
                LoginGoogleModel loginModel = new() { Gmail = email, ProviderKey = providerKey };
                try
                {
                    ApplicationUser? userLoginGoogle = await userService.CreateUserLoginGoogle(loginModel);
                    (string token, IEnumerable<string> roles) = authService.GenerateJwtToken(userLoginGoogle);
                    string refreshToken = authService.
                    return Ok(BaseResponse<object>.OkResponse(new
                    {
                        access_token = token,
                        token_type = "JWT",
                        auth_type = "Bearer",
                        expires_in = DateTime.UtcNow.AddHours(1),
                        user = new
                        {
                            email = userLoginGoogle.Email,
                            role = roles
                        }
                    }));
                }
                catch (BaseException.ErrorException ex)
                {
                    return StatusCode(ex.StatusCode, new BaseException.ErrorException(ex.StatusCode, ex.ErrorDetail.ErrorCode, ex.ErrorDetail.ErrorMessage.ToString()));

                }
            }
            catch
            {
                return BadRequest(new BaseException.ErrorException(400, "BadRequest", "Token không hợp lệ"));
            }

        }



    }
}
