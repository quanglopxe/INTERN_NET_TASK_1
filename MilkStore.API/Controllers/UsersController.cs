using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core;
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

        [HttpGet("Get_User")]
        [Authorize]
        public async Task<IActionResult> GetUsers(string? id, int index = 1, int pageSize = 10)
        {

            IEnumerable<UserResponeseDTO> users = await _userService.GetUser(id, index, pageSize);
            return Ok(BaseResponse<IEnumerable<UserResponeseDTO>>.OkResponse(users));
        }

        [HttpPost("Add_User_With_Role_Async")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddUserWithRoleAsync(UserModelView userModel)
        {
            await _userService.AddUserWithRoleAsync(userModel);
            return Ok(BaseResponse<object>.OkResponse("Create user successfully"));
        }

        [HttpPut("User_Update")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateModelView userModel)
        {
            await _userService.UpdateUser(userModel);
            return Ok(BaseResponse<object>.OkResponse("Update user successfully"));
        }
        [HttpPatch("User_Update_By_Admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserByAdmin([FromQuery] string userId, [FromBody] UserUpdateByAdminModel userModel)
        {
            await _userService.UpdateUserByAdmin(userId, userModel);
            return Ok(BaseResponse<object>.OkResponse("Update user successfully"));
        }

        [HttpDelete("Delete_One_User")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOneUser([FromQuery] string userId)
        {

            await _userService.DeleteUser(userId);
            return Ok(BaseResponse<object>.OkResponse("Delete user successfully"));
        }


        [HttpGet("Get_User_Profile")]
        [Authorize]
        public async Task<IActionResult> GetUserProfile()
        {
            UserProfileResponseModelView? userProfile = await _userService.GetUserProfile();
            return Ok(BaseResponse<UserProfileResponseModelView>.OkResponse(userProfile));
        }
        [HttpGet("Get_User_By_Role")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetUserByRole(string roleId, int index = 1, int pageSize = 10)
        {
            BasePaginatedList<UserResponeseDTO>? users = await _userService.GetUserByRole(roleId, index, pageSize);
            return Ok(BaseResponse<BasePaginatedList<UserResponeseDTO>>.OkResponse(users));
        }
    }
}
