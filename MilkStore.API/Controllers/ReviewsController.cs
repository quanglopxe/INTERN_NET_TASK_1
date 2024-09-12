using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core.Base;
using MilkStore.ModelViews.PostModelViews;
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
        [HttpGet("GetReviews")]
        public async Task<IActionResult> GetReviews(string? id)
        {
            IList<Review> reviews = (IList<Review>)await _reviewsService.GetReviews(id);
            return Ok(BaseResponse<IList<Review>>.OkResponse(reviews));
        }
        [HttpPost("CreateReviews")]
        public async Task<IActionResult> CreateReviews(ReviewsModel reviewsModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseException.BadRequestException("BadRequest", ModelState.ToString()));
            }
            Review review = await _reviewsService.CreateReviews(reviewsModel);
            return Ok(BaseResponse<Review>.OkResponse(review));
        }
        [HttpPut("UpdateReviews")]
        public async Task<IActionResult> UpdateReviews(string id, ReviewsModel reviewsModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseException.BadRequestException("BadRequest", ModelState.ToString()));
            }
            Review review = await _reviewsService.UpdateReviews(id, reviewsModel);
            return Ok(BaseResponse<Review>.OkResponse(review));
        }
        [HttpDelete("DeleteReviews")]
        public async Task<IActionResult> DeleteReviews(string id)
        {
            await _reviewsService.DeleteReviews(id);
            return Ok();
        }
    }
}
