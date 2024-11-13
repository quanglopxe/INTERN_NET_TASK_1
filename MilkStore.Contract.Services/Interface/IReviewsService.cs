using MilkStore.Contract.Repositories.Entity;
using MilkStore.Core;
using MilkStore.ModelViews.PreOrdersModelView;
using MilkStore.ModelViews.ProductsModelViews;
using MilkStore.ModelViews.ResponseDTO;
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
        Task<BasePaginatedList<ReviewResponseDTO>> GetAsync(string? id, string? productName, int pageIndex, int pageSize);
        Task CreateReviews(ReviewsModel reviewsModel);
        Task UpdateReviews(string id, ReviewsModel reviewsModel);
        Task DeletReviews(string id);
    }
}
