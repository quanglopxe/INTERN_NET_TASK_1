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
        Task<BasePaginatedList<PostResponseDTO>> GetPosts(string? id, int index, int pageSize);
        Task<PostResponseDTO> CreatePost(PostModelView postModel);        
        Task<PostResponseDTO> UpdatePost(string id, PostModelView postModel);
        Task DeletePost(string id);
    }
}
