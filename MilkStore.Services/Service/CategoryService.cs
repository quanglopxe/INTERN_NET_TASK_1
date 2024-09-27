using AutoMapper;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Repositories.Interface;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core;
using MilkStore.Core.Base;
using MilkStore.Core.Constants;
using MilkStore.ModelViews.CategoryModelViews;
using MilkStore.ModelViews.ProductsModelViews;
using MilkStore.Repositories.Context;

namespace MilkStore.Services.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DatabaseContext context;
        private readonly IMapper _mapper;
        public CategoryService(DatabaseContext context, IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.context = context;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task CreateCategory(CategoryModel CategoryModel)
        {
            if (CategoryModel.Id.Contains(" "))
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, "Error!!! Input wrong id");
            }
            if (CategoryModel.Id == "" || CategoryModel.Id == null)
            {
                CategoryModel.Id = Guid.NewGuid().ToString("N");
            }
            Category newCategory = _mapper.Map<Category>(CategoryModel);
            newCategory.CreatedTime = DateTime.UtcNow;

            await _unitOfWork.GetRepository<Category>().InsertAsync(newCategory);
            await _unitOfWork.SaveAsync();
        }
        public async Task DeleteCategory(string id)
        {
            if(string.IsNullOrWhiteSpace(id))
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, "Error!!! Input wrong id");
            }    
            Category Category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(id);

            if (Category.DeletedTime != null)
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, $"Doesn't exist:{id}");
            }
            Category.DeletedTime = DateTime.UtcNow;
            await _unitOfWork.GetRepository<Category>().UpdateAsync(Category);
            await _unitOfWork.SaveAsync();
        }


        public async Task<IEnumerable<CategoryModel>> GetCategory(string? id)
        {
            if (id == null)
            {
                // Lấy tất cả sản phẩm
                IEnumerable<Category> Category = await _unitOfWork.GetRepository<Category>().GetAllAsync();

                // Lọc sản phẩm có DeleteTime == null
                Category = Category.Where(p => p.DeletedTime == null);

                return _mapper.Map<IEnumerable<CategoryModel>>(Category);
            }
            else
            {
                // Lấy sản phẩm theo ID
                Category product = await _unitOfWork.GetRepository<Category>().GetByIdAsync(id);

                if (product != null && product.DeletedTime == null) // Kiểm tra DeleteTime
                {
                    return new List<CategoryModel> { _mapper.Map<CategoryModel>(product) };
                }
                else
                {
                    return new List<CategoryModel>();
                }
            }
        }


        public async Task UpdateCategory(string id, CategoryModel CategoryModel)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, "Error!!! Input wrong id");
            }
            Category existingCategory = await _unitOfWork.GetRepository<Category>().GetByIdAsync(id);

            if (existingCategory == null)
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, $"Doesn't exist:{id}");
            }

            // Cập nhật thông tin sản phẩm bằng cách ánh xạ từ DTO
            _mapper.Map(CategoryModel, existingCategory);
            existingCategory.LastUpdatedTime = DateTime.UtcNow;

            await _unitOfWork.GetRepository<Category>().UpdateAsync(existingCategory);
            await _unitOfWork.SaveAsync();
        }


    }
}
