using Microsoft.EntityFrameworkCore;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Repositories.Interface;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core.Utils;
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

        public ReviewsService(IUnitOfWork unitOfWork, DatabaseContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }
        public async Task<Review> CreateReviews(ReviewsModel reviewsModel)
        {
            var newReviews = new Review
            {
                ProductID = reviewsModel.ProductID,
                UserID = reviewsModel.UserID,
                Rating = reviewsModel.Rating,
                Comment = reviewsModel.Comment,
            };
            await _unitOfWork.GetRepository<Review>().InsertAsync(newReviews);
            await _unitOfWork.SaveAsync();
            return newReviews;
        }

        public async Task DeleteReviews(string id)
        {
            var review = await _unitOfWork.GetRepository<Review>().GetByIdAsync(id);
            if (review == null)
            {
                throw new KeyNotFoundException($"Review with ID {id} was not found.");
            }
            review.DeletedTime = CoreHelper.SystemTimeNow;
            await _unitOfWork.GetRepository<Review>().UpdateAsync(review);
            await _unitOfWork.SaveAsync();

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
                throw new KeyNotFoundException($"Review with ID {id} was not found.");
            }
            review.Rating = reviewsModel.Rating;
            review.Comment = reviewsModel.Comment;
            review.LastUpdatedTime = CoreHelper.SystemTimeNow;
            await _unitOfWork.GetRepository<Review>().UpdateAsync(review);
            await _unitOfWork.SaveAsync();
            return review;
        }
    }
}
