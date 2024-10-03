using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core.Base;
using MilkStore.ModelViews.ResponseDTO;
using MilkStore.ModelViews.UserModelViews;

namespace MilkStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpGet("GetUser")]
        [Authorize]
        public async Task<IActionResult> GetUsers(string? id, int index = 1, int pageSize = 10)
        {

            IEnumerable<UserResponeseDTO> users = await _userService.GetUser(id, index, pageSize);
            return Ok(BaseResponse<IEnumerable<UserResponeseDTO>>.OkResponse(users));
        }

        [HttpPost("AddUserWithRoleAsync")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddUserWithRoleAsync(UserModelView userModel)
        {
            await _userService.AddUserWithRoleAsync(userModel);
            return Ok(BaseResponse<object>.OkResponse("Create user successfully"));
        }

        [HttpPut("UserUpdate")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateModelView userModel)
        {
            await _userService.UpdateUser(userModel);
            return Ok(BaseResponse<object>.OkResponse("Update user successfully"));
        }

        [HttpDelete("DeleteOneUser")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOneUser([FromQuery] string userId)
        {

            await _userService.DeleteUser(userId);
            return Ok(BaseResponse<object>.OkResponse("Delete user successfully"));
        }
    }
}
