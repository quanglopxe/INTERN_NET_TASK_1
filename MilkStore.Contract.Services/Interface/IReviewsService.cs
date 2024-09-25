using MilkStore.Contract.Repositories.Entity;
using MilkStore.ModelViews.PreOrdersModelView;
using MilkStore.ModelViews.ProductsModelViews;
using MilkStore.ModelViews.ReviewsModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.Contract.Services.Interface
{
    public interface IReviewsService
    {
        Task<IEnumerable<Review>> GetReviews(string? id, int page, int pageSize);
        Task CreateReviews(ReviewsModel reviewsModel, string userID, string userEmail);
        Task<Review> UpdateReviews(string id, ReviewsModel reviewsModel, string userID);
        Task DeletReviews(string id);
    }
}
