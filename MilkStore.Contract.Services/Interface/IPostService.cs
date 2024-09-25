using Microsoft.AspNetCore.Identity;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Core;
using MilkStore.ModelViews.PostModelViews;
using MilkStore.ModelViews.ResponseDTO;
using MilkStore.Repositories.Entity;

namespace MilkStore.Contract.Services.Interface
{
    public interface IPostService
    {
        Task<BasePaginatedList<PostResponseDTO>> GetPosts(string? id, string? name, int index, int pageSize);
        Task CreatePost(PostModelView postModel, string userID);        
        Task UpdatePost(string id, PostModelView postModel, string userID);
        Task DeletePost(string id);
    }
}
