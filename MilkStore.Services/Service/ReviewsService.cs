using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Repositories.Interface;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core.Utils;
using MilkStore.ModelViews.ReviewsModelView;
using MilkStore.Repositories.Context;
using MilkStore.Services.EmailSettings;
using System.Security.Claims;

namespace MilkStore.Services.Service
{
    public class ReviewsService : IReviewsService
    {
        private readonly IUnitOfWork _unitOfWork;        
        private readonly IMapper _mapper;
        private readonly EmailService _emailService;
        public ReviewsService(IUnitOfWork unitOfWork, IMapper mapper)
        {            
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task CreateReviews(ReviewsModel reviewsModel, string userID, string userEmail)
        {
            Order order = await _unitOfWork.GetRepository<Order>().GetByIdAsync(reviewsModel.OrderID);            
            if (order == null || order.UserId.ToString() != userID)
            {
                throw new UnauthorizedAccessException("You do not have access to this order.");
            }
            var productInOrder = order.OrderDetailss.Where(od => od.ProductID.Contains(reviewsModel.ProductsID)).FirstOrDefault();
            if (productInOrder == null)
            {
                throw new KeyNotFoundException($"Product with ID {reviewsModel.ProductsID} was not found in this order.");
            }
            Review newReview = _mapper.Map<Review>(reviewsModel);
            newReview.CreatedTime = DateTime.UtcNow;
            newReview.CreatedBy = userID;
            await _unitOfWork.GetRepository<Review>().InsertAsync(newReview);
            await _unitOfWork.SaveAsync();

            // Gửi email phản hồi
            if (!string.IsNullOrEmpty(userEmail))
            {
                var subject = "Cảm ơn bạn đã đánh giá sản phẩm!";
                var body = $@"
            <p>Xin chào,</p>
            <p>Cảm ơn bạn đã đánh giá sản phẩm {productInOrder.Products.ProductName}. Đánh giá của bạn giúp chúng tôi cải thiện dịch vụ.</p>
            <p>Thông tin đánh giá:</p>
            <ul>
                <li>Sản phẩm: {productInOrder.Products.ProductName}</li>
                <li>Đánh giá: {reviewsModel.Rating} sao</li>
                <li>Nhận xét: {reviewsModel.Comment}</li>
            </ul>
            <p>Trân trọng,<br>MilkStore</p>
        ";
                await _emailService.SendEmailAsync(userEmail, subject, body);
            }
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

        public async Task<Review> UpdateReviews(string id, ReviewsModel reviewsModel, string userID)
        {

            Review review = await _unitOfWork.GetRepository<Review>().GetByIdAsync(id);            
            if (review == null)
            {
                throw new Exception($"Review have ID: {id} was not found.");
            }

            _mapper.Map(reviewsModel, review);
            review.LastUpdatedTime = DateTime.UtcNow;
            review.LastUpdatedBy = userID;
            await _unitOfWork.GetRepository<Review>().UpdateAsync(review);
            await _unitOfWork.SaveAsync();
            return review;
        }
    }
}
