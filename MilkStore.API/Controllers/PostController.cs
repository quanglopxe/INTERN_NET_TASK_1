﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core;
using MilkStore.Core.Base;
using MilkStore.ModelViews.PostModelViews;
using MilkStore.ModelViews.ResponseDTO;
using System.Security.Claims;


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
        public async Task<IActionResult> GetPost(string? id, string? name, int index = 1, int pageSize = 10)
        {
            try
            {
                BasePaginatedList<PostResponseDTO>? paginatedPosts = await _postService.GetPosts(id, name, index, pageSize);
                return Ok(BaseResponse<BasePaginatedList<PostResponseDTO>>.OkResponse(paginatedPosts));
            }
            catch (BaseException.ErrorException e)
            {

                return StatusCode(e.StatusCode, new BaseException.ErrorException(e.StatusCode, e.ErrorDetail.ErrorCode, e.ErrorDetail.ErrorMessage.ToString()));
            }

        }
        [Authorize(Roles = "Staff")]
        [HttpPost()]
        public async Task<IActionResult> CreatePost(PostModelView postModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseException.BadRequestException("BadRequest", ModelState.ToString()));
            }
            try
            {
                //HttpContext
                string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if(string.IsNullOrWhiteSpace(userId))
                {
                    return BadRequest(new BaseException.BadRequestException("BadRequest", "Vui lòng đăng nhập!"));
                }
                await _postService.CreatePost(postModel, userId);
                return Ok(BaseResponse<string>.OkResponse("Thêm bài viết thành công!"));
            }
            catch (BaseException.ErrorException e)
            {

                return StatusCode(e.StatusCode, new BaseException.ErrorException(e.StatusCode, e.ErrorDetail.ErrorCode, e.ErrorDetail.ErrorMessage.ToString()));
            }
            
        }
        [Authorize(Roles = "Staff")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(string id, PostModelView postModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseException.BadRequestException("BadRequest", ModelState.ToString()));
            }
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _postService.UpdatePost(id, postModel, userId);
                return Ok(BaseResponse<string>.OkResponse("Sửa bài viết thành công!"));                
            }
            catch (BaseException.ErrorException e)
            {

                return StatusCode(e.StatusCode, new BaseException.ErrorException(e.StatusCode, e.ErrorDetail.ErrorCode, e.ErrorDetail.ErrorMessage.ToString()));
            }
            
        }
        [Authorize(Roles = "Staff")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(string id)
        {
            try
            {
                await _postService.DeletePost(id);
                return Ok(BaseResponse<string>.OkResponse("Xóa thành công!"));
            }
            catch (BaseException.ErrorException e)
            {

                return StatusCode(e.StatusCode, new BaseException.ErrorException(e.StatusCode, e.ErrorDetail.ErrorCode, e.ErrorDetail.ErrorMessage.ToString()));

            }            
        }
    }
}
