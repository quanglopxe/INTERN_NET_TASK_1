using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Repositories.Interface;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core;
using MilkStore.Core.Base;
using MilkStore.Core.Constants;
using MilkStore.Core.Utils;
using MilkStore.ModelViews.ResponseDTO;
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
            string? userId = _httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string? userEmail = _httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.Email)?.Value;
            
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(userEmail))
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.Unauthorized, ErrorCode.Unauthorized, "Vui lòng đăng nhập vào bằng tài khoản đã xác thực!");
            }
            
            OrderDetails? orderDetail = await _unitOfWork.GetRepository<OrderDetails>()
                .Entities
                .AsNoTracking()
                .FirstOrDefaultAsync(od => od.ProductID == reviewsModel.ProductsID
                                           && od.Order.UserId.ToString() == userId
                                           && od.Order.OrderStatuss == OrderStatus.Delivered
                                           && od.Order.PaymentStatuss == PaymentStatus.Paid);

            if (orderDetail == null)
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.NotFound, ErrorCode.NotFound, $"Không tìm thấy sản phẩm đã mua với mã {reviewsModel.ProductsID} trong đơn hàng của bạn!");
            }
            
            Review newReview = _mapper.Map<Review>(reviewsModel);
            newReview.UserID = Guid.Parse(userId);            
            newReview.OrderID = orderDetail.OrderID;
            newReview.CreatedTime = CoreHelper.SystemTimeNow;
            newReview.CreatedBy = userId;
            newReview.OrderDetailID = orderDetail.Id;
            await _unitOfWork.GetRepository<Review>().InsertAsync(newReview);
            await _unitOfWork.SaveAsync();

            if (!string.IsNullOrEmpty(userEmail))
            {
                var subject = "Cảm ơn bạn đã đánh giá sản phẩm!";
                var body = $@"
            <p>Xin chào,</p>
            <p>Cảm ơn bạn đã đánh giá sản phẩm {orderDetail.Products.ProductName}. Đánh giá của bạn giúp chúng tôi cải thiện dịch vụ.</p>
            <p>Thông tin đánh giá:</p>
            <ul>
                <li>Sản phẩm: {orderDetail.Products.ProductName}</li>
                <li>Đánh giá: {reviewsModel.Rating} sao</li>
                <li>Nhận xét: {reviewsModel.Comment}</li>
            </ul>
            <p>Trân trọng,<br>MilkStore</p>";

                await _emailService.SendEmailAsync(userEmail, subject, body);
            }
        }


        public async Task DeletReviews(string id)
        {
            Review review = await _unitOfWork.GetRepository<Review>().GetByIdAsync(id);
            if (review == null)
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.NotFound, ErrorCode.BadRequest, $"Không tìm thấy đánh giá nào có mã {id}!!");
            }
            review.DeletedTime = CoreHelper.SystemTimeNow;
            review.DeletedBy = "Admin";
            await _unitOfWork.GetRepository<Review>().UpdateAsync(review);
            await _unitOfWork.SaveAsync();
        }

        public async Task<BasePaginatedList<ReviewResponseDTO>> GetAsync(string? id, string? productName, int pageIndex, int pageSize)
        {
            IQueryable<Review>? query = _unitOfWork.GetRepository<Review>()
                .Entities
                .AsNoTracking()
                .Include(rv => rv.OrderDetails) 
                .ThenInclude(od => od.Products) 
                .Where(rv => rv.DeletedTime == null);

            if (!string.IsNullOrWhiteSpace(id))
            {
                query = query.Where(rv => rv.Id == id);
            }
            if (!string.IsNullOrWhiteSpace(productName))
            {
                query = query.Where(rv => rv.OrderDetails != null && rv.OrderDetails.Products != null && rv.OrderDetails.Products.ProductName == productName);
            }

            BasePaginatedList<Review>? paginatedReviews = await _unitOfWork.GetRepository<Review>()
                .GetPagging(query, pageIndex, pageSize);

            if (!paginatedReviews.Items.Any() && !string.IsNullOrWhiteSpace(id))
            {
                Review? rvById = await query.FirstOrDefaultAsync();
                if (rvById != null)
                {
                    ReviewResponseDTO? rvDto = _mapper.Map<ReviewResponseDTO>(rvById);
                    return new BasePaginatedList<ReviewResponseDTO>(new List<ReviewResponseDTO> { rvDto }, 1, 1, 1);
                }
            }

            List<ReviewResponseDTO>? rvDtosResult = _mapper.Map<List<ReviewResponseDTO>>(paginatedReviews.Items);
            return new BasePaginatedList<ReviewResponseDTO>(
                rvDtosResult,
                paginatedReviews.TotalItems,
                paginatedReviews.CurrentPage,
                paginatedReviews.PageSize
            );
        }



        public async Task UpdateReviews(string id, ReviewsModel reviewsModel)
        {

            Review? review = await _unitOfWork.GetRepository<Review>().GetByIdAsync(id);            
            if (review == null)
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.NotFound, ErrorCode.NotFound, $"Không tìm thấy đánh giá nào có mã {id}!!");
            }
            string userID = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (string.IsNullOrWhiteSpace(userID))
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.Unauthorized, ErrorCode.Unauthorized, "Vui lòng đăng nhập vào bằng tài khoản đã xác thực!");
            }
            if (review.ProductsID != reviewsModel.ProductsID)
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.NotFound, "ProductID không khớp với sản phẩm được đánh giá!");
            }
            _mapper.Map(reviewsModel, review);            
            review.LastUpdatedTime = CoreHelper.SystemTimeNow;            
            review.LastUpdatedBy = userID;
            await _unitOfWork.GetRepository<Review>().UpdateAsync(review);
            await _unitOfWork.SaveAsync();

        }
    }
}
