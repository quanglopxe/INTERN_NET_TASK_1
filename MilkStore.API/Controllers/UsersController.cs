using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core.Base;
using MilkStore.ModelViews.UserModelViews;
using MilkStore.Repositories.Entity;
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
        public async Task<IActionResult> Login(int index = 1, int pageSize = 10)
        {
            // IList<UserResponseModel> a = await _userService.GetAll();
            // return Ok(BaseResponse<IList<UserResponseModel>>.OkResponse(a));
            return Ok("ok");
        }
    }
}
