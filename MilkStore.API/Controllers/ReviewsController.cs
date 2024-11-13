using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core;
using MilkStore.Core.Base;
using MilkStore.Core.Constants;
using MilkStore.ModelViews.ResponseDTO;
using MilkStore.ModelViews.ReviewsModelView;


namespace MilkStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewsService _reviewsService;
        public ReviewsController(IReviewsService reviewsService, IMapper mapper)
        {
            _reviewsService = reviewsService;
        }
        [HttpGet]
        public async Task<IActionResult> GetReviews(string? id, string? productName,int page = 1, int pageSize = 10)
        {
            BasePaginatedList<ReviewResponseDTO> reviews = await _reviewsService.GetAsync(id, productName, page, pageSize);
            return Ok(BaseResponse<BasePaginatedList<ReviewResponseDTO>>.OkResponse(reviews));
        }
        [HttpPost()]
        [Authorize(Roles = "Member, Staff")]
        public async Task<IActionResult> CreateReviews(ReviewsModel reviewsModel)
        {            
            await _reviewsService.CreateReviews(reviewsModel);
            return Ok(BaseResponse<string>.OkResponse("Thêm đánh giá thành công!"));
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Member, Staff")]
        public async Task<IActionResult> UpdateReview(string id, [FromBody] ReviewsModel reviewsModel)
        {
            await _reviewsService.UpdateReviews(id, reviewsModel);
            return Ok(BaseResponse<string>.OkResponse("Sửa đánh giá thành công!"));            
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Member , Staff")]
        public async Task<IActionResult> DeleteReview(string id)
        {
            await _reviewsService.DeletReviews(id);
            return Ok(BaseResponse<string>.OkResponse("Xóa đánh giá thành công!"));
        }
    }
}

