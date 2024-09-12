using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Repositories.Interface;
using MilkStore.Contract.Services.Interface;
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

        public async Task<Products> CreateProducts(ProductsModel ProductsModel)
        {
            var newProducts = new Products
            {
                ProductName = ProductsModel.ProductName,
                Description = ProductsModel.Description,
                Price = ProductsModel.Price,
                QuantityInStock = ProductsModel.QuantityInStock,
                ImageUrl = ProductsModel.ImageUrl,
            };
            await _unitOfWork.GetRepository<Products>().InsertAsync(newProducts);
            await _unitOfWork.SaveAsync();
            return newProducts;
        }

        public async Task<Products> DeleteProducts(object id)
        {
            var product = await _unitOfWork.GetRepository<Products>().GetByIdAsync(id);

            if (product == null)
            {
                throw new Exception("Sản phẩm không tồn tại.");
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
                var products = await _unitOfWork.GetRepository<Products>().GetByIdAsync(id);
                return products != null ? new List<Products> { products } : new List<Products>();
            }

        }

        public async Task<Products> UpdateProducts(string id,ProductsModel ProductsModel)
        {

            var existingProduct = await _unitOfWork.GetRepository<Products>().GetByIdAsync(id);

            if (existingProduct == null)
            {
                throw new Exception("Sản phẩm không tồn tại.");
            }

            existingProduct.ProductName = ProductsModel.ProductName;
            existingProduct.Description = ProductsModel.Description;
            existingProduct.Price = ProductsModel.Price;
            existingProduct.QuantityInStock = ProductsModel.QuantityInStock;
            existingProduct.ImageUrl = ProductsModel.ImageUrl;
            //existingProduct.LastUpdatedTime = DateTime.UtcNow;
            
            await _unitOfWork.GetRepository<Products>().UpdateAsync(obj: existingProduct);

            await _unitOfWork.SaveAsync();

            return existingProduct;
        }
    }
}
