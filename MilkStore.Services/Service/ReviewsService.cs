using Microsoft.EntityFrameworkCore;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Repositories.Interface;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core.Utils;
using MilkStore.ModelViews.PreOrdersModelView;
using MilkStore.ModelViews.ProductsModelViews;
using MilkStore.ModelViews.ReviewsModelView;
using MilkStore.Repositories.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.Services.Service
{
    public class ReviewsService : IReviewsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DatabaseContext _context;
        public ReviewsService(DatabaseContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<Review> CreateReviews(ReviewsModel reviewsModel)
        {
            var newReview = new Review
            {
                ProductID = reviewsModel.ProductID,
                UserID = reviewsModel.UserID,
                Rating = reviewsModel.Rating,
                Comment = reviewsModel.Comment,
            };
            await _unitOfWork.GetRepository<Review>().InsertAsync(newReview);
            await _unitOfWork.SaveAsync();
            return newReview;
        }

        public async Task<Review> DeletReviews(string id)
        {
            var review = await _unitOfWork.GetRepository<Review>().GetByIdAsync(id);

            if (review == null)
            {
                throw new Exception("Review không tồn tại.");
            }

            await _unitOfWork.GetRepository<Review>().DeleteAsync(id);
            await _unitOfWork.SaveAsync();
            return review;
        }

        public async Task<IEnumerable<Review>> GetReviews(string? id)
        {
            if (id == null)
            {
                return await _unitOfWork.GetRepository<Review>().GetAllAsync();
            }
            else
            {
                var reviews = await _unitOfWork.GetRepository<Review>().GetByIdAsync(id);
                return reviews != null ? new List<Review> { reviews } : new List<Review>();
            }

        }

        public async Task<Review> UpdateReviews(string id, ReviewsModel reviewsModel)
        {

            var review = await _unitOfWork.GetRepository<Review>().GetByIdAsync(id);

            if (review == null)
            {
                throw new Exception("Pre-order không tồn tại.");
            }

            review.ProductID = reviewsModel.ProductID;
            review.UserID = reviewsModel.UserID;
            review.Rating = reviewsModel.Rating;
            review.Comment = reviewsModel.Comment;
            review.LastUpdatedTime = DateTime.UtcNow;
            await _unitOfWork.GetRepository<Review>().UpdateAsync(review);
            await _unitOfWork.SaveAsync();
            return review;
        }
        public async Task<IList<ReviewsModel>> Pagination(int pageSize, int pageNumber)
        {
            var skip = (pageNumber - 1) * pageSize;
            return await _context.Reviews.Skip(skip).Take(pageSize).Select(r => new ReviewsModel
            {
                ProductID = r.ProductID,
                UserID = r.UserID,
                Rating = r.Rating,
                Comment = r.Comment,
            }).ToArrayAsync();
        }
    }
}
