﻿using Microsoft.AspNetCore.Identity;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.ModelViews.PostModelViews;
using MilkStore.Repositories.Entity;

namespace MilkStore.Contract.Services.Interface
{
    public interface IPostService
    {
        Task<IEnumerable<Post>> GetPosts(string? id);
        Task<Post> CreatePost(PostModelView postModel);
        Task<Post> UpdatePost(PostModelView postModel);
        Task<Post> DeletePost(string id);
    }
}
