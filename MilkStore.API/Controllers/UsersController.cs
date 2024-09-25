using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core.Base;
using MilkStore.ModelViews.ResponseDTO;
using MilkStore.ModelViews.UserModelViews;
using MilkStore.Repositories.Entity;
using System.Security.Claims;
namespace MilkStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(IUserService userService, UserManager<ApplicationUser> userManager)
        {
            _userService = userService;
            _userManager = userManager;

        }
        [HttpGet()]
        public async Task<IActionResult> GetUsers(string? id, int index = 1, int pageSize = 10)
        {
            try
            {
                IEnumerable<UserResponeseDTO> users = await _userService.GetUser(id, index, pageSize);
                return Ok(BaseResponse<IEnumerable<UserResponeseDTO>>.OkResponse(users));
            }
            catch (BaseException.ErrorException e)
            {
                return StatusCode(e.StatusCode, new BaseException.ErrorException(e.StatusCode, e.ErrorDetail.ErrorCode, e.ErrorDetail.ErrorMessage.ToString()));
            }
        }
        [HttpPost("add")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddUser(UserModelView userModel)
        {
            string createdBy = User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "System";
            try
            {
                ApplicationUser newUser = await _userService.AddUser(userModel, createdBy);
                if (newUser is null)
                {
                    return BadRequest(new { message = "Failed to create user" });
                }
                return Ok(new { message = "Successfully created user" });
            }
            catch (BaseException.ErrorException e)
            {
                return StatusCode(e.StatusCode, new BaseException.ErrorException(e.StatusCode, e.ErrorDetail.ErrorCode, e.ErrorDetail.ErrorMessage.ToString()));
            }
        }

        //[HttpGet()]
        //public async Task<IActionResult> Login(int index = 1, int pageSize = 10)
        //{
        //    // IList<UserResponseModel> a = await _userService.GetAll();
        //    // return Ok(BaseResponse<IList<UserResponseModel>>.OkResponse(a));
        //    return Ok("ok");
        //}
        [HttpPut("update/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserModelView userModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                string updatedBy = User.Identity?.Name ?? "System";
                ApplicationUser updatedUser = await _userService.UpdateUser(id, userModel, updatedBy);
                return Ok(new { message = "Successfully update user" });
            }
            catch (BaseException.ErrorException e)
            {
                return StatusCode(e.StatusCode, new BaseException.ErrorException(e.StatusCode, e.ErrorDetail.ErrorCode, e.ErrorDetail.ErrorMessage.ToString()));
            }
        }

        [HttpDelete("delete/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete1User(Guid userId)
        {

            try
            {
                string deleteby = User.Identity?.Name ?? "System";
                ApplicationUser deletedUser = await _userService.DeleteUser(userId, deleteby);
                return Ok(new { message = "Successfully deleted user" });
            }
            catch (BaseException.ErrorException e)
            {
                return StatusCode(e.StatusCode, new BaseException.ErrorException(e.StatusCode, e.ErrorDetail.ErrorCode, e.ErrorDetail.ErrorMessage.ToString()));
            }
        }
    }
}
