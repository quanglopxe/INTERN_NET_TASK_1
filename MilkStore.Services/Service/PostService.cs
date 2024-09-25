// using AutoMapper;
// using Microsoft.AspNetCore.Http;
// using Microsoft.EntityFrameworkCore;
// using MilkStore.Contract.Repositories.Entity;
// using MilkStore.Contract.Repositories.Interface;
// using MilkStore.Contract.Services.Interface;
// using MilkStore.Core;
// using MilkStore.Core.Base;
// using MilkStore.Core.Constants;
// using MilkStore.Core.Utils;
// using MilkStore.ModelViews.PostModelViews;
// using MilkStore.ModelViews.ResponseDTO;

// namespace MilkStore.Services.Service
// {
//     public class PostService : IPostService
//     {

//         private readonly IUnitOfWork _unitOfWork;
//         private readonly IMapper _mapper;

//         public PostService(IUnitOfWork unitOfWork, IMapper mapper)
//         {
//             _unitOfWork = unitOfWork;
//             _mapper = mapper;            
//         }        
//         public async Task CreatePost(PostModelView postModel, string userID)
//         {            
//             Post newPost = _mapper.Map<Post>(postModel);
//             newPost.CreatedTime = CoreHelper.SystemTimeNow;
//             newPost.DeletedTime = null;
//             newPost.CreatedBy = userID;

//             // Thêm sản phẩm vào bài đăng bằng PostProduct
//             if (postModel.ProductIDs != null && postModel.ProductIDs.Any())
//             {
//                 newPost.PostProducts = new List<PostProduct>();

//                 foreach (string productId in postModel.ProductIDs)
//                 {
//                     Products? product = await _unitOfWork.GetRepository<Products>().GetByIdAsync(productId);
//                     if (product != null && product.DeletedTime == null)
//                     {
//                         newPost.PostProducts.Add(new PostProduct
//                         {
//                             Post = newPost,
//                             Product = product
//                         });
//                     }
//                     else if (product == null)
//                     {
//                         throw new BaseException.ErrorException(StatusCodes.NotFound, "NotFound", $"Product with ID {productId} was not found.");
//                     }
//                     else
//                     {
//                         throw new BaseException.ErrorException(401, "BadRequest", $"Product with ID {productId} has been deleted.");
//                     }
//                 }
//             }
//             await _unitOfWork.GetRepository<Post>().InsertAsync(newPost);
//             await _unitOfWork.SaveAsync();
//         }

//         public async Task DeletePost(string id)
//         {
//             if (string.IsNullOrWhiteSpace(id))
//             {
//                 throw new BaseException.ErrorException(400, "BadRequest", "Post ID is required.");
//             }
//             Post? post = await _unitOfWork.GetRepository<Post>().GetByIdAsync(id)
//                  ?? throw new BaseException.ErrorException(404, "NotFound", $"Post with ID {id} was not found.");
//             if (post.DeletedTime != null)
//             {
//                 throw new BaseException.ErrorException(400, "BadRequest", $"Post with ID {id} has already been deleted.");
//             }
//             post.DeletedTime = CoreHelper.SystemTimeNow;
//             await _unitOfWork.GetRepository<Post>().UpdateAsync(post);
//             await _unitOfWork.SaveAsync();
//         }

//         public async Task<BasePaginatedList<PostResponseDTO>> GetPosts(string? id, string? name, int pageIndex, int pageSize)
//         {
//             IQueryable<Post>? query = _unitOfWork.GetRepository<Post>().Entities.Where(post => post.DeletedTime == null);
//             if (!string.IsNullOrWhiteSpace(id))
//             {
//                 query = query.Where(post => post.Id == id);
//             }
//             if (!string.IsNullOrWhiteSpace(name))
//             {
//                 query = query.Where(post => post.Title.Contains(name));
//             }
//             BasePaginatedList<Post>? paginatedPosts = await _unitOfWork.GetRepository<Post>().GetPagging(query, pageIndex, pageSize);

//             if (!paginatedPosts.Items.Any())
//             {
//                 if (!string.IsNullOrWhiteSpace(id))
//                 {
//                     Post? postById = await _unitOfWork.GetRepository<Post>().Entities
//                         .FirstOrDefaultAsync(post => post.Id == id && post.DeletedTime == null);
//                     if (postById != null)
//                     {
//                         PostResponseDTO? postDto = _mapper.Map<PostResponseDTO>(postById);
//                         return new BasePaginatedList<PostResponseDTO>(new List<PostResponseDTO> { postDto }, 1, 1, 1);
//                     }
//                 }

//                 if (!string.IsNullOrWhiteSpace(name))
//                 {
//                     List<Post>? postsByName = await _unitOfWork.GetRepository<Post>().Entities
//                         .Where(post => post.Title.Contains(name) && post.DeletedTime == null)
//                         .ToListAsync();
//                     if (postsByName.Any())
//                     {
//                         List<PostResponseDTO>? paginatedPostDtos = _mapper.Map<List<PostResponseDTO>>(postsByName);
//                         return new BasePaginatedList<PostResponseDTO>(paginatedPostDtos, 1, 1, postsByName.Count());
//                     }
//                 }
//             }

//             //GetAll
//             List<PostResponseDTO>? postDtosResult = _mapper.Map<List<PostResponseDTO>>(paginatedPosts.Items);

//             return new BasePaginatedList<PostResponseDTO>(
//                 postDtosResult,
//                 paginatedPosts.TotalItems,
//                 paginatedPosts.CurrentPage,
//                 paginatedPosts.PageSize
//             );
//         }

//         public async Task UpdatePost(string id, PostModelView postModel, string userID)
//         {
//             if (string.IsNullOrWhiteSpace(id))
//             {
//                 throw new BaseException.ErrorException(400, "BadRequest", "Post ID is required.");
//             }
//             Post? post = await _unitOfWork.GetRepository<Post>().GetByIdAsync(id)
//              ?? throw new BaseException.ErrorException(404, "NotFound", $"Post with ID {id} was not found.");          

//             //map từ PostModelView sang Post (chỉ cập nhật các trường thay đổi)
//             _mapper.Map(postModel, post);

//             post.LastUpdatedTime = CoreHelper.SystemTimeNow;
//             post.LastUpdatedBy = userID;

//             // Kiểm tra xem sản phẩm có bị xóa chưa, thêm sản phẩm nếu cần
//             if (postModel.ProductIDs != null && postModel.ProductIDs.Any())
//             {
//                 post.PostProducts = new List<PostProduct>();

//                 foreach (string productId in postModel.ProductIDs)
//                 {
//                     Products? product = await _unitOfWork.GetRepository<Products>().GetByIdAsync(productId);
//                     if (product != null)
//                     {
//                         post.PostProducts.Add(new PostProduct
//                         {
//                             Post = post,
//                             Product = product
//                         });
//                     }
//                     else
//                     {
//                         throw new BaseException.ErrorException(404, "NotFound", $"Product with ID {productId} was not found.");
//                     }
//                 }
//             }
//             await _unitOfWork.GetRepository<Post>().UpdateAsync(post);
//             await _unitOfWork.SaveAsync();
//         }
//     }
// }
