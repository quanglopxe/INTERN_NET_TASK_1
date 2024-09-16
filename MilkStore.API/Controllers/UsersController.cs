using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Services.Interface;
using MilkStore.ModelViews.UserModelViews;
using MilkStore.Repositories.Entity;
using MilkStore.Services.Service;
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
        public async Task<IEnumerable<User>> GetUsers(Guid? id, int index = 1, int pageSize = 10)
        {
            var users = await _userService.GetUser(id);
            return users;
        }
        //[HttpGet()]
        //public async Task<IActionResult> Login(int index = 1, int pageSize = 10)
        //{
        //    // IList<UserResponseModel> a = await _userService.GetAll();
        //    // return Ok(BaseResponse<IList<UserResponseModel>>.OkResponse(a));
        //    return Ok("ok");
        //}
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserModelView userModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedBy = User.Identity?.Name ?? "System";
            var updatedUser = await _userService.UpdateUser(id, userModel, updatedBy);

            if (updatedUser == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(updatedUser);
        }

        [HttpDelete("delete/{userId}")]
        public async Task<IActionResult> Delete1User(string userId)
        {
            var createdBy = User.Identity?.Name ?? "System";

            var deletedUser = await _userService.DeleteUser(userId, createdBy);
            if (deletedUser == null)
            {
                return NotFound(new { message = "User not found" });
            }
            return Ok(deletedUser);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddUser(UserModelView userModel)
        {
            var createdBy = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "System";

            var newUser = await _userService.AddUser(userModel, createdBy);

            if (newUser == null)
            {
                return BadRequest(new { message = "Failed to create user" });
            }

            return Ok(newUser);
        }
    }
}
