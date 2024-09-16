﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core.Base;
using MilkStore.ModelViews.PostModelViews;
using MilkStore.ModelViews.ResponseDTO;
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
            IList<PostResponseDTO> posts = (IList<PostResponseDTO>)await _postService.GetPosts(id);
            return Ok(BaseResponse<IList<PostResponseDTO>>.OkResponse(posts));
        }
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(string id)
        {
            await _postService.DeletePost(id);
            return Ok();
        }
    }
}
