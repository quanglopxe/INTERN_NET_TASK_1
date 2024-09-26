﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Repositories.Interface;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core.Base;
using MilkStore.Core.Constants;
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
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ReviewsService(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService, IHttpContextAccessor httpContextAccessor)
        {            
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task CreateReviews(ReviewsModel reviewsModel)
        {            
            OrderDetails? orderDetail = await _unitOfWork.GetRepository<OrderDetails>().GetByIdAsync(reviewsModel.OrderDetailID);
            if (orderDetail == null)
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, $"Order details not found with {reviewsModel.OrderDetailID}!!");
            }
                        
            Order? order = await _unitOfWork.GetRepository<Order>().GetByIdAsync(orderDetail.OrderID);
            string userID = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            string userEmail = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email).Value;

            if(string.IsNullOrWhiteSpace(userID) || string.IsNullOrWhiteSpace(userEmail))
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, "Please log in with an email verified account!");
            }
            if (order == null || order.UserId.ToString() != userID)
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, "You do not have access to review this product!");
            }
            var productInOrder = order.OrderDetailss.Where(od => od.ProductID.Contains(orderDetail.ProductID)).FirstOrDefault();
            if (productInOrder == null)
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, $"Order details not found for {orderDetail.ProductID}!!");
            }
            Review newReview = _mapper.Map<Review>(reviewsModel);
            newReview.UserID = Guid.Parse(userID);
            newReview.ProductsID = orderDetail.ProductID;
            newReview.OrderID = orderDetail.OrderID;
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
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, $"No reviews found for {id}!!");
            }
            review.DeletedTime = CoreHelper.SystemTimeNow;
            review.DeletedBy = "Admin";
            await _unitOfWork.GetRepository<Review>().UpdateAsync(review);
            await _unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<ReviewsModel>> GetReviews(string? id, int page, int pageSize)
        {
            if (id == null)
            {
                var query = _unitOfWork.GetRepository<Review>()
                    .Entities
                    .Where(detail => detail.DeletedTime == null)
                    .OrderBy(detail => detail.ProductsID);


                var paginated = await _unitOfWork.GetRepository<Review>()
                .GetPagging(query, page, pageSize);

                return _mapper.Map<IEnumerable<ReviewsModel>>(paginated.Items);
            }
            else
            {
                var review = await _unitOfWork.GetRepository<Review>()
                    .Entities
                    .FirstOrDefaultAsync(r => r.Id == id && r.DeletedTime == null);
                if (review == null)
                {
                    throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, $"No reviews found for {id}!!");
                }
                return _mapper.Map<List<ReviewsModel>>(new List<Review> { review});
            }
        }

        public async Task<Review> UpdateReviews(string id, ReviewsModel reviewsModel)
        {

            Review? review = await _unitOfWork.GetRepository<Review>().GetByIdAsync(id);            
            if (review == null)
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, $"No reviews found for {id}!!");
            }
            string userID = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (string.IsNullOrWhiteSpace(userID))
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, "Please log in with an authenticated account!");
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
