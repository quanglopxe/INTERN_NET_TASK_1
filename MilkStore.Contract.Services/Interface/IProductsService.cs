using MilkStore.Contract.Repositories.Entity;
using MilkStore.Core;
using MilkStore.ModelViews.ProductsModelViews;

namespace MilkStore.Contract.Services.Interface
{
    public interface IProductsService
    {
        Task<BasePaginatedList<ProductsModel>> GetProducts(string? id, int pageIndex, int pageSize);
        Task CreateProducts(ProductsModel productsModel);
        Task UpdateProducts(string id, ProductsModel productsModel);
        Task DeleteProducts(string id);
        Task<IEnumerable<ProductsModel>> GetProductsName(string? ProductdName, string? CategoryName);
    }
}
