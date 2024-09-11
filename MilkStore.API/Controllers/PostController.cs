using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core.Base;
using MilkStore.ModelViews.PostModelViews;
using MilkStore.ModelViews.UserModelViews;
using MilkStore.Repositories.Entity;

namespace MilkStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;        
        public PostController(IPostService postService)
        {
            _postService = postService;            
        }
        [HttpGet()]
        public async Task<IActionResult> GetPost(string? id, int index = 1, int pageSize = 10)
        {            
            IList<Post> posts = (IList<Post>)await _postService.GetPosts(id);
            return Ok(BaseResponse<IList<Post>>.OkResponse(posts));
        }
        [HttpPost()]
        public async Task<IActionResult> CreatePost(PostModelView postModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseException.BadRequestException("BadRequest", ModelState.ToString()));
            }
            Post post = await _postService.CreatePost(postModel);
            return Ok(BaseResponse<Post>.OkResponse(post));
        }        
    }
}
