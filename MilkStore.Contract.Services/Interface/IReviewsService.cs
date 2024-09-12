﻿using MilkStore.Contract.Repositories.Entity;
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
        Task<IEnumerable<Review>> GetReviews(string? id);
        Task<Review> CreateReviews(ReviewsModel reviewsModel);
        Task<Review> UpdateReviews(string id, ReviewsModel reviewsModel);
        Task DeleteReviews(string id);
    }
}
