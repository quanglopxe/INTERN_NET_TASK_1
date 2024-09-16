using Microsoft.AspNetCore.Identity;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.ModelViews.PostModelViews;
using MilkStore.ModelViews.ResponseDTO;
using MilkStore.Repositories.Entity;

namespace MilkStore.Contract.Services.Interface
{
    public interface IPostService
    {
        Task<IEnumerable<PostResponseDTO>> GetPosts(string? id);
        Task<PostResponseDTO> CreatePost(PostModelView postModel);        
        Task<PostResponseDTO> UpdatePost(string id, PostModelView postModel);
        Task DeletePost(string id);
    }
}
