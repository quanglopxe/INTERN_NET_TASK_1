using Microsoft.AspNetCore.Identity;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Repositories.Interface;
using MilkStore.Contract.Services.Interface;
using MilkStore.ModelViews.AuthModelViews;
using MilkStore.ModelViews.PostModelViews;
using MilkStore.ModelViews.UserModelViews;
using MilkStore.Repositories.Context;
using MilkStore.Repositories.Entity;

namespace MilkStore.Services.Service
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DatabaseContext context;
        public PostService(DatabaseContext context, IUnitOfWork unitOfWork)
        {
            this.context = context;                        
            _unitOfWork = unitOfWork;
        }

        public async Task<Post> CreatePost(PostModelView postModel)
        {
            var newPost = new Post
            {
                Title = postModel.Title,
                Content = postModel.Content,   
                Image = postModel.Image
            };
            await _unitOfWork.GetRepository<Post>().InsertAsync(newPost);
            await _unitOfWork.SaveAsync();
            return newPost;
        }

        public async Task<Post> DeletePost(string id)
        {
            throw new NotImplementedException();

        }

        public async Task<IEnumerable<Post>> GetPosts(string? id)
        {
            if(id == null)
            {
                return await _unitOfWork.GetRepository<Post>().GetAllAsync();
            }
            else
            {
                var post = await _unitOfWork.GetRepository<Post>().GetByIdAsync(id);
                return post != null ? new List<Post> { post } : new List<Post>();
            }            

        }

        public async Task<Post> UpdatePost(PostModelView postModel)
        {
            throw new NotImplementedException();
        }
    }
}
