﻿using Microsoft.AspNetCore.Mvc;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core.Base;
using XuongMay.ModelViews.AuthModelViews;

namespace XuongMayBE.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly IUserService userService;
        public AuthController(IAuthService authService, IUserService userService)
        {
            this.authService = authService;
            this.userService = userService;
        }

        [HttpGet("auth_account")]
        public async Task<IActionResult> Login(LoginModelView model)
        {
            var result = await authService.CheckUser(model.Username);
            if (result == null)
            {
                return Unauthorized("Không tìm thấy người dùng");
            }
            var resultPassword = await authService.CheckPassword(model);
            if (!resultPassword.Succeeded)
            {
                return Unauthorized("Không đúng mật khẩu");
            }
            var token = authService.GenerateJwtToken(result);
            return Ok(BaseResponse<object>.OkResponse(new
            {
                access_token = token,
                token_type = "JWT",
                auth_type = "Bearer",
                expires_in = DateTime.UtcNow.AddHours(1),
                user = new
                {
                    userName = result.UserName,
                    email = result.Email
                }
            }));
        }

        [HttpPost("new_account")]
        public async Task<IActionResult> Register(RegisterModelView model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseException.BadRequestException("BadRequest", ModelState.ToString()));
            }
            var existingUser = await userService.GetUserByEmail(model.Email);
            if (existingUser != null)
            {
                return Conflict(new BaseException.ErrorException(409, "Conflict", "User đã tồn tại!"));
            }
            var result = await userService.CreateUser(model);
            if (result.Succeeded)
            {
                return Ok(BaseResponse<string>.OkResponse("Tạo thành công"));
            }
            return StatusCode(500, new BaseException.ErrorException(500, "InternalServerError", "Lỗi khi tạo người dùng"));
        }
    }
}
