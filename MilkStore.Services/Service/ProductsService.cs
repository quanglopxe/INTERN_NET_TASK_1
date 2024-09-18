using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Repositories.Interface;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core;
using MilkStore.ModelViews.ProductsModelViews;
using MilkStore.Repositories.Context;

namespace MilkStore.Services.Service
{
    public class ProductsService : IProductsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DatabaseContext context;
        public ProductsService(DatabaseContext context, IUnitOfWork unitOfWork)
        {
            this.context = context;
            _unitOfWork = unitOfWork;
        }
        public async Task<BasePaginatedList<Products>> PagingProducts(int pageIndex, int pageSize)
        {
            IQueryable<Products> query = _unitOfWork.GetRepository<Products>().Entities;
            // Sử dụng hàm GetPagging để lấy danh sách phân trang
            BasePaginatedList<Products> paginatedList = await _unitOfWork.GetRepository<Products>().GetPagging(query, pageIndex, pageSize);
            //return new BasePaginatedList<T>(items, count, index, pageSize);
            return paginatedList; // Trả về danh sách phân trang
        }
        public async Task<Products> CreateProducts(ProductsModel ProductsModel)
        {
            Products newProducts = new Products
            {
                ProductName = ProductsModel.ProductName,
                Description = ProductsModel.Description,
                Price = ProductsModel.Price,
                QuantityInStock = ProductsModel.QuantityInStock,
                ImageUrl = ProductsModel.ImageUrl,
                CreatedTime = DateTime.UtcNow,
                //CreatedBy = userName,
            };
            await _unitOfWork.GetRepository<Products>().InsertAsync(newProducts);
            await _unitOfWork.SaveAsync();
            return newProducts;
        }

        public async Task<Products> DeleteProducts(object id)
        {
            Products product = await _unitOfWork.GetRepository<Products>().GetByIdAsync(id);

            if (product.DeletedTime != null)
            {
                throw new Exception($"Mã hàng đã được xóa:{id}");
            }

            await _unitOfWork.GetRepository<Products>().DeleteAsync(id);

            await _unitOfWork.SaveAsync();

            return product;

        }

        public async Task<IEnumerable<Products>> GetProducts(string? id)
        {
            if (id == null)
            {
                return await _unitOfWork.GetRepository<Products>().GetAllAsync();
            }
            else
            {
                Products products = await _unitOfWork.GetRepository<Products>().GetByIdAsync(id);
                return products != null ? new List<Products> { products } : new List<Products>();
            }

        }

        public async Task<Products> UpdateProducts(string id,ProductsModel ProductsModel)
        {

            Products existingProduct = await _unitOfWork.GetRepository<Products>().GetByIdAsync(id);

            if (existingProduct == null)
            {
                throw new Exception("Sản phẩm không tồn tại.");
            }

            existingProduct.ProductName = ProductsModel.ProductName;
            existingProduct.Description = ProductsModel.Description;
            existingProduct.Price = ProductsModel.Price;
            existingProduct.QuantityInStock = ProductsModel.QuantityInStock;
            existingProduct.ImageUrl = ProductsModel.ImageUrl;
            existingProduct.LastUpdatedTime = DateTime.UtcNow;

            await _unitOfWork.GetRepository<Products>().UpdateAsync(obj: existingProduct);

            await _unitOfWork.SaveAsync();

            return existingProduct;
        }
    }
}
