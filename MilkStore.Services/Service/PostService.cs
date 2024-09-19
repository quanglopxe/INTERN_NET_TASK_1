using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Repositories.Interface;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core;
using MilkStore.Core.Utils;
using MilkStore.ModelViews.PostModelViews;
using MilkStore.ModelViews.ResponseDTO;
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

        public async Task<PostResponseDTO> CreatePost(PostModelView postModel)
        {
            Post newPost = new Post
            {
                Title = postModel.Title,
                Content = postModel.Content,
                Image = postModel.Image,
                CreatedTime = CoreHelper.SystemTimeNow,
                LastUpdatedTime = CoreHelper.SystemTimeNow                
            };
            // Thêm sản phẩm vào bài đăng bằng PostProduct
            if (postModel.ProductIDs != null && postModel.ProductIDs.Any())
            {
                newPost.PostProducts = new List<PostProduct>();

                foreach (var productId in postModel.ProductIDs)
                {
                    //check deletetime

                    var product = await _unitOfWork.GetRepository<Products>().GetByIdAsync(productId);
                    if (product != null)
                    {
                        newPost.PostProducts.Add(new PostProduct
                        {
                            Post = newPost,
                            Product = product
                        });
                    }
                    else
                    {
                        throw new KeyNotFoundException($"Product with ID {productId} was not found.");
                    }
                }
            }
            await _unitOfWork.GetRepository<Post>().InsertAsync(newPost);
            await _unitOfWork.SaveAsync();
            return MapToPostResponseDto(newPost);
        }
        private PostResponseDTO MapToPostResponseDto(Post post)
        {
            return new PostResponseDTO
            {
                PostID = post.Id,
                Title = post.Title,
                Content = post.Content,
                CreatedAt = post.CreatedTime,
                CreatedBy = post.CreatedBy,
                Products = post.PostProducts.Select(pp => new ProductResponseDTO
                {
                    ProductID = pp.Product.Id,
                    ProductName = pp.Product.ProductName,
                    Description = pp.Product.Description,
                    Price = pp.Product.Price,
                    QuantityInStock = pp.Product.QuantityInStock,
                    ImageUrl = pp.Product.ImageUrl
                }).ToList()
            };
        }
        public async Task DeletePost(string id)
        {            
            Post post = await _unitOfWork.GetRepository<Post>().GetByIdAsync(id);            
            if (post == null)
            {
                throw new KeyNotFoundException($"Post with ID {id} was not found.");
            }
            if (post.DeletedTime != null)
            {
                throw new InvalidOperationException($"Post with ID {id} has already been deleted.");
            }
            post.DeletedTime = CoreHelper.SystemTimeNow;
            await _unitOfWork.GetRepository<Post>().UpdateAsync(post);
            await _unitOfWork.SaveAsync();

        }
        //tên
        //public async Task<BasePaginatedList<PostResponseDTO>> GetPosts(string? id, string? name, int pageIndex, int pageSize)
        //{
        //    //rule code
        //    if(id.IsNullOrEmpty() && name.IsNullOrEmpty())
        //    {
        //        var query = _unitOfWork.GetRepository<Post>().Entities.Where(post => post.DeletedTime == null);
        //        var paginatedPosts = await _unitOfWork.GetRepository<Post>().GetPagging(query, pageIndex, pageSize);

        //        // Ánh xạ paginatedPosts sang kiểu trả về khác
        //        var paginatedPostDtos = new BasePaginatedList<PostResponseDTO>(
        //            paginatedPosts.Items.Select(MapToPostResponseDto).ToList(),
        //            paginatedPosts.TotalPages,
        //            paginatedPosts.CurrentPage,
        //            paginatedPosts.PageSize
        //            );
        //        return paginatedPostDtos;
        //    }
        //    if (!id.IsNullOrEmpty())
        //    {
        //        Post post = await _unitOfWork.GetRepository<Post>().Entities.FirstOrDefaultAsync(post => post.Id == id && post.DeletedTime == null);
        //        if (post == null)
        //        {
        //            throw new KeyNotFoundException($"Post with ID {id} was not found.");
        //        }

        //        var postDto = new List<PostResponseDTO> { MapToPostResponseDto(post) };
        //        return new BasePaginatedList<PostResponseDTO>(postDto, 1, 1, 1);
        //    }
        //    if (!name.IsNullOrEmpty())
        //    {
        //        var query = _unitOfWork.GetRepository<Post>().Entities.Where(post => post.Title.Contains(name) && post.DeletedTime == null);
        //        var paginatedPosts = await _unitOfWork.GetRepository<Post>().GetPagging(query, pageIndex, pageSize);

        //        // Ánh xạ paginatedPosts sang kiểu trả về khác
        //        var paginatedPostDtos = new BasePaginatedList<PostResponseDTO>(
        //            paginatedPosts.Items.Select(MapToPostResponseDto).ToList(),
        //            paginatedPosts.TotalPages,
        //            paginatedPosts.CurrentPage,
        //            paginatedPosts.PageSize
        //            );
        //        return paginatedPostDtos;
        //    }

        //}
        public async Task<BasePaginatedList<PostResponseDTO>> GetPosts(string? id, string? name, int pageIndex, int pageSize)
        {            
            var query = _unitOfWork.GetRepository<Post>().Entities.Where(post => post.DeletedTime == null);
            if (!id.IsNullOrEmpty())
            {
                query = query.Where(post => post.Id == id);
            }
            if (!name.IsNullOrEmpty())
            {
                query = query.Where(post => post.Title.Contains(name));
            }
            var paginatedPosts = await _unitOfWork.GetRepository<Post>().GetPagging(query, pageIndex, pageSize);

            if (!paginatedPosts.Items.Any())
            {
                if (!id.IsNullOrEmpty())
                {
                    var postById = await _unitOfWork.GetRepository<Post>().Entities
                        .FirstOrDefaultAsync(post => post.Id == id && post.DeletedTime == null);
                    if (postById != null)
                    {
                        var postDto = new List<PostResponseDTO> { MapToPostResponseDto(postById) };
                        return new BasePaginatedList<PostResponseDTO>(postDto, 1, 1, 1);
                    }
                }

                if (!name.IsNullOrEmpty())
                {
                    var postsByName = await _unitOfWork.GetRepository<Post>().Entities
                        .Where(post => post.Title.Contains(name) && post.DeletedTime == null)
                        .ToListAsync();
                    if (postsByName.Any())
                    {
                        var paginatedPostDtos = new BasePaginatedList<PostResponseDTO>(
                            postsByName.Select(MapToPostResponseDto).ToList(),
                            1, // TotalPages
                            1, // CurrentPage
                            postsByName.Count // PageSize
                        );
                        return paginatedPostDtos;
                    }
                }
            }

            // Ánh xạ paginatedPosts sang kiểu trả về khác
            var paginatedPostDtosResult = new BasePaginatedList<PostResponseDTO>(
                paginatedPosts.Items.Select(MapToPostResponseDto).ToList(),
                paginatedPosts.TotalPages,
                paginatedPosts.CurrentPage,
                paginatedPosts.PageSize
            );

            return paginatedPostDtosResult;
        }
        //kiểm tra trùng với db trước khi update        
        public async Task<PostResponseDTO> UpdatePost(string id, PostModelView postModel)
        {
            Post post = await _unitOfWork.GetRepository<Post>().GetByIdAsync(id);
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
            return MapToPostResponseDto(post);
        }

        //Task<BasePaginatedList<PostResponseDTO>> IPostService.GetPosts(string? id, int index, int pageSize)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
