using MilkStore.Contract.Repositories.Entity;
using MilkStore.Core;
using MilkStore.ModelViews.ProductsModelViews;

namespace MilkStore.Contract.Services.Interface
{
    public interface IProductsService
    {
        Task<BasePaginatedList<ProductsModel>> GetProducts(string? id, int pageIndex, int pageSize);
        Task<Products> CreateProducts(ProductsModel productsModel);
        Task<Products> UpdateProducts(string id, ProductsModel productsModel);
        Task<Products> DeleteProducts(object id);
        Task<IEnumerable<ProductsModel>> GetProductsName(string? ProductdName, string? CategoryName);
    }
}
