using Microsoft.EntityFrameworkCore;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Repositories.Interface;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core.Utils;
using MilkStore.ModelViews.PostModelViews;
using MilkStore.Repositories.Context;
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
                Image = postModel.Image,
                CreatedTime = CoreHelper.SystemTimeNow,
                LastUpdatedTime = CoreHelper.SystemTimeNow,
                DeletedTime = null
            };
            await _unitOfWork.GetRepository<Post>().InsertAsync(newPost);
            await _unitOfWork.SaveAsync();
            return newPost;
        }

        public async Task DeletePost(string id)
        {
            var post = await _unitOfWork.GetRepository<Post>().GetByIdAsync(id);
            if (post == null)
            {
                throw new KeyNotFoundException($"Post with ID {id} was not found.");
            }
            post.DeletedTime = CoreHelper.SystemTimeNow;
            await _unitOfWork.GetRepository<Post>().UpdateAsync(post);
            await _unitOfWork.SaveAsync();

        }

        public async Task<IEnumerable<Post>> GetPosts(string? id)
        {
            if (id == null)
            {
                return await _unitOfWork.GetRepository<Post>().Entities.Where(post => post.DeletedTime == null).ToListAsync();
            }
            else
            {
                var post = await _unitOfWork.GetRepository<Post>().Entities.FirstOrDefaultAsync(post => post.Id == id && post.DeletedTime == null);
                if (post == null)
                {
                    throw new KeyNotFoundException($"Post with ID {id} was not found.");
                }
                return new List<Post> { post };
            }

        }

        public async Task<Post> UpdatePost(string id, PostModelView postModel)
        {
            var post = await _unitOfWork.GetRepository<Post>().GetByIdAsync(id);
            if (post == null)
            {
                throw new KeyNotFoundException($"Post with ID {id} was not found.");
            }
            post.Title = postModel.Title;
            post.Content = postModel.Content;
            post.Image = postModel.Image;
            post.DeletedTime = postModel.DeletedTime;
            post.LastUpdatedTime = CoreHelper.SystemTimeNow;
            await _unitOfWork.GetRepository<Post>().UpdateAsync(post);
            await _unitOfWork.SaveAsync();
            return post;
        }
    }
}
