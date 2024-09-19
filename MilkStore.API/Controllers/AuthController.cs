﻿using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
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
                    return Ok(BaseResponse<object>.OkResponse(new
                    {
                        access_token = token,
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
            // AuthenticateResult result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // if (!result.Succeeded)
            // {
            //     return Unauthorized(new BaseException.ErrorException(401, "Unauthorized", "Xác thực không thành công"));
            // }
            // IEnumerable<Claim> claims = result.Principal.Claims;

            // LoginGoogleModel loginModel = new()
            // {
            //     Gmail = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
            //     ProviderKey = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
            // };
            // ApplicationUser existingEmail = await userService.GetUserByEmail(loginModel.Gmail);
            // if (existingEmail != null)
            // {
            //     if (existingEmail.DeletedTime.HasValue)
            //     {
            //         return BadRequest(new BaseException.ErrorException(400, "BadRequest", "Tài khoản đã bị xóa"));
            //     }
            //     (string token, IEnumerable<string> roles) = authService.GenerateJwtToken(existingEmail);
            //     return Ok(BaseResponse<object>.OkResponse(new
            //     {
            //         access_token = token,
            //         token_type = "JWT",
            //         auth_type = "Bearer",
            //         expires_in = DateTime.UtcNow.AddHours(1),
            //         user = new
            //         {
            //             email = existingEmail.Email,
            //             role = roles
            //         }
            //     }));

            // }
            // else
            // {
            //     IdentityResult createUserGoogle = await userService.CreateUserLoginGoogle(loginModel);
            //     if (createUserGoogle.Succeeded)
            //     {
            //         (string token, IEnumerable<string> roles) = authService.GenerateJwtToken(existingEmail);
            //         return Ok(BaseResponse<object>.OkResponse(new
            //         {
            //             access_token = token,
            //             token_type = "JWT",
            //             auth_type = "Bearer",
            //             expires_in = DateTime.UtcNow.AddHours(1),
            //             user = new
            //             {
            //                 email = existingEmail.Email,
            //                 role = roles
            //             }
            //         }));
            //     }
            //     else
            //     {
            //         return StatusCode(500, new BaseException.ErrorException(500, "InternalServerError", $"Lỗi khi tạo người dùng {createUserGoogle.Errors.FirstOrDefault()?.Description}"));
            //     }
            // }
            /// fix sau
            return Ok();
        }
    }
}
