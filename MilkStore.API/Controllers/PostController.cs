using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core;
using MilkStore.Core.Base;
using MilkStore.ModelViews.PostModelViews;
using MilkStore.ModelViews.ResponseDTO;
using MilkStore.ModelViews.UserModelViews;
using MilkStore.Repositories.Context;
using MilkStore.Repositories.Entity;

namespace MilkStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly DatabaseContext _context;
        public PostController(IPostService postService, DatabaseContext context)
        {
            _postService = postService;            
        }        
        [HttpGet()]
        public async Task<IActionResult> GetPost(string? id, int index = 1, int pageSize = 5)
        {                        
            IList<PostResponseDTO> posts = (IList<PostResponseDTO>)await _postService.GetPosts(id);
            int totalItems = posts.Count;
            var pagedPosts = posts.Skip((index - 1) * pageSize).Take(pageSize).ToList();

            // Tạo danh sách phân trang
            var paginatedList = new BasePaginatedList<PostResponseDTO>(pagedPosts, totalItems, index, pageSize);

            return Ok(BaseResponse<BasePaginatedList<PostResponseDTO>>.OkResponse(paginatedList));
        }
        [Authorize(Roles = "Staff")]
        [HttpPost()]
        public async Task<IActionResult> CreatePost(PostModelView postModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseException.BadRequestException("BadRequest", ModelState.ToString()));
            }
            PostResponseDTO post = await _postService.CreatePost(postModel);
            return Ok(BaseResponse<PostResponseDTO>.OkResponse(post));
        }
        [Authorize(Roles = "Staff")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(string id, PostModelView postModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseException.BadRequestException("BadRequest", ModelState.ToString()));
            }
            PostResponseDTO post = await _postService.UpdatePost(id, postModel);
            return Ok(BaseResponse<PostResponseDTO>.OkResponse(post));
        }
        [Authorize(Roles = "Staff")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(string id)
        {
            await _postService.DeletePost(id);
            return Ok();
        }
    }
}
