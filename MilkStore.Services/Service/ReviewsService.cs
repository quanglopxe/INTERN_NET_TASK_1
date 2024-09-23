using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Repositories.Interface;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core.Utils;
using MilkStore.ModelViews.ReviewsModelView;
using MilkStore.Repositories.Context;
using System.Security.Claims;

namespace MilkStore.Services.Service
{
    public class ReviewsService : IReviewsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public ReviewsService(IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task CreateReviews(ReviewsModel reviewsModel)
        {
            Order order = await _unitOfWork.GetRepository<Order>().GetByIdAsync(reviewsModel.OrderID);
            var userID = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (order == null || order.UserId.ToString() != userID)
            {
                throw new UnauthorizedAccessException("You do not have access to this order.");
            }
            var productInOrder = order.OrderDetailss.Where(od => od.ProductID.Contains(reviewsModel.ProductID)).FirstOrDefault();
            if (productInOrder == null)
            {
                throw new KeyNotFoundException($"Product with ID {reviewsModel.ProductID} was not found in this order.");
            }
            Review newReview = _mapper.Map<Review>(reviewsModel);
            newReview.CreatedTime = DateTime.UtcNow;
            await _unitOfWork.GetRepository<Review>().InsertAsync(newReview);
            await _unitOfWork.SaveAsync();            
        }

        public async Task DeletReviews(string id)
        {
            Review review = await _unitOfWork.GetRepository<Review>().GetByIdAsync(id);
            if (review == null)
            {
                throw new KeyNotFoundException($"Review with ID {id} was not found.");
            }
            review.DeletedTime = CoreHelper.SystemTimeNow;
            await _unitOfWork.GetRepository<Review>().UpdateAsync(review);
            await _unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<Review>> GetReviews(string? id, int page, int pageSize)
        {
            if (id == null)
            {
                var query = _unitOfWork.GetRepository<Review>()
                    .Entities
                    .Where(detail => detail.DeletedTime == null)
                    .OrderBy(detail => detail.ProductsID);


                var paginated = await _unitOfWork.GetRepository<Review>()
                .GetPagging(query, page, pageSize);

                return paginated.Items;

            }
            else
            {
                var review = await _unitOfWork.GetRepository<Review>()
                    .Entities
                    .FirstOrDefaultAsync(r => r.Id == id && r.DeletedTime == null);
                if (review == null)
                {
                    throw new KeyNotFoundException($"Review have ID: {id} was not found.");
                }
                return _mapper.Map<List<Review>>(review);
            }

        }

        public async Task<Review> UpdateReviews(string id, ReviewsModel reviewsModel)
        {

            Review review = await _unitOfWork.GetRepository<Review>().GetByIdAsync(id);

            if (review == null)
            {
                throw new Exception($"Review have ID: {id} was not found.");
            }

            //review.ProductId = reviewsModel.ProductId;
            //review.UserId = reviewsModel.UserId;
            //review.Rating = reviewsModel.Rating;
            //review.Comment = reviewsModel.Comment;
            _mapper.Map(reviewsModel, review);
            review.LastUpdatedTime = DateTime.UtcNow;
            await _unitOfWork.GetRepository<Review>().UpdateAsync(review);
            await _unitOfWork.SaveAsync();
            return review;
        }
    }
}
