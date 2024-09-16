﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core.Base;
using MilkStore.ModelViews.PostModelViews;
using MilkStore.ModelViews.PreOrdersModelView;
using MilkStore.ModelViews.ReviewsModelView;
using MilkStore.Services.Service;

namespace MilkStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewsService _reviewsService;
        public ReviewsController(IReviewsService reviewsService)
        {
            _reviewsService = reviewsService;
        }
        [HttpGet]
        [Authorize(Roles = "Admin, Member")]
        public async Task<IActionResult> GetReviews(string? id)
        {
            try
            {
                var reviews = await _reviewsService.GetReviews(id);

                if (reviews == null || !reviews.Any())
                {
                    return NotFound("Reviewkhông tồn tại!!!");
                }

                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }
        [HttpPost()]
        [Authorize(Roles = "Admin, Member")]
        public async Task<IActionResult> CreateReviews(ReviewsModel reviewsModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseException.BadRequestException("BadRequest", ModelState.ToString()));
            }
            Review Reviews = await _reviewsService.CreateReviews(reviewsModel);
            return Ok(BaseResponse<Review>.OkResponse(Reviews));
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Member")]
        public async Task<IActionResult> UpdateReview(string id, [FromBody] ReviewsModel reviewsModel)
        {
            if (!ModelState.IsValid)
            {
                return NotFound("Review không tồn tại!!!");
            }

            try
            {
                var updatedReview = await _reviewsService.UpdateReviews(id, reviewsModel);
                return Ok(updatedReview);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Member")]
        public async Task<IActionResult> DeleteReview(string id)
        {
            try
            {
                var deletedReview = await _reviewsService.DeletReviews(id);
                return Ok(deletedReview);
            }
            catch (Exception ex)
            {
                return NotFound("Review không tồn tại!!!");
            }
        }
        [HttpGet("Pagination")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Pagination([FromQuery] int pageSize, [FromQuery] int pageNumber)
        {
            var result = await _reviewsService.Pagination(pageSize, pageNumber);
            return Ok(result);
        }
    }
}
