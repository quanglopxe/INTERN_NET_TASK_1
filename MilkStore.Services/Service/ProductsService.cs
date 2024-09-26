using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Repositories.Interface;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core;
using MilkStore.Core.Utils;
using MilkStore.ModelViews.ProductsModelViews;
using MilkStore.Repositories.Context;

namespace MilkStore.Services.Service
{
    public class ProductsService : IProductsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DatabaseContext context;
        private readonly IMapper _mapper;
        public ProductsService(DatabaseContext context, IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.context = context;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<ProductsModel>> GetProductsName(string? ProductdName, string? CategoryName)
        {
            IEnumerable<Products> products = await _unitOfWork.GetRepository<Products>().GetAllAsync();
            if (CategoryName == null)
            {
                products = products.Where(p => p.ProductName.Contains(ProductdName, StringComparison.OrdinalIgnoreCase) && p.DeletedTime == null);
            }
            if (ProductdName == null)
            {
                string temp = "";
                IEnumerable<Category> cte = await _unitOfWork.GetRepository<Category>().GetAllAsync();
                foreach (Category c in cte)
                {
                    if (c.CategoryName == CategoryName)
                    {
                        temp = c.Id;
                    }
                }
                products = products.Where(p => p.CategoryId.Contains(temp, StringComparison.OrdinalIgnoreCase) && p.DeletedTime == null);
            }
            return _mapper.Map<IEnumerable<ProductsModel>>(products);
        }
        public async Task<Products> CreateProducts(ProductsModel productsModel)
        {
            Products newProduct = _mapper.Map<Products>(productsModel);
            newProduct.CreatedTime = DateTime.UtcNow;

            await _unitOfWork.GetRepository<Products>().InsertAsync(newProduct);
            await _unitOfWork.SaveAsync();

            return newProduct;
        }
        public async Task<Products> DeleteProducts(object id)
        {
            Products product = await _unitOfWork.GetRepository<Products>().GetByIdAsync(id);

            if (product.DeletedTime != null)
            {
                throw new Exception($"Mã hàng đã được xóa:{id}");
            }
            product.DeletedTime = DateTime.UtcNow;
            await _unitOfWork.GetRepository<Products>().UpdateAsync(product);
            await _unitOfWork.SaveAsync();

            return product;
        }


        public async Task<BasePaginatedList<ProductsModel>> GetProducts(string? id, int pageIndex, int pageSize)
        {
            IQueryable<Products> query = _unitOfWork.GetRepository<Products>().Entities;

            // Kiểm tra xem có truyền id hay không
            if (!string.IsNullOrEmpty(id))
            {
                // Lấy sản phẩm theo id
                query = query.Where(p => p.Id == id && p.DeletedTime == null);

                // Nếu chỉ cần tìm kiếm theo id thì không cần phân trang, trả về 1 kết quả hoặc rỗng
                var product = await query.FirstOrDefaultAsync();
                if (product != null)
                {
                    var productModel = _mapper.Map<ProductsModel>(product);
                    return new BasePaginatedList<ProductsModel>(new List<ProductsModel> { productModel }, 1, 1, 1); // Chỉ trả về 1 sản phẩm
                }
                else
                {
                    return new BasePaginatedList<ProductsModel>(new List<ProductsModel>(), 0, pageIndex, pageSize); // Trả về rỗng
                }
            }

            // Nếu không có id thì lấy tất cả sản phẩm và áp dụng phân trang
            query = query.Where(p => p.DeletedTime == null);

            // Lấy danh sách phân trang
            BasePaginatedList<Products> paginatedList = await _unitOfWork.GetRepository<Products>().GetPagging(query, pageIndex, pageSize);

            // Ánh xạ sang ProductsModel và trả về kết quả phân trang
            var productsModel = _mapper.Map<IEnumerable<ProductsModel>>(paginatedList.Items);
            return new BasePaginatedList<ProductsModel>(productsModel.ToList(), paginatedList.TotalPages, pageIndex, pageSize);
        }


        private async Task UpdatePreOrdersDeletedTime(string productId)
        {
            var preOrders = await _unitOfWork.GetRepository<PreOrders>()
                .Entities
                .Where(p => p.ProductID == productId && p.DeletedTime == null)
                .ToListAsync();

            foreach (var preOrder in preOrders)
            {
                preOrder.DeletedTime = CoreHelper.SystemTimeNow;
                await _unitOfWork.GetRepository<PreOrders>().UpdateAsync(preOrder);
            }

            await _unitOfWork.SaveAsync();
        }

        public async Task<Products> UpdateProducts(string id, ProductsModel productsModel)
        {
            Products product = await _unitOfWork.GetRepository<Products>().GetByIdAsync(id);
            if (product == null)
            {
                throw new KeyNotFoundException($"Sản phẩm với ID {id} không tồn tại.");
            }
            var oldQuantityInStock = product.QuantityInStock;

            _mapper.Map(productsModel, product);
            product.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<Products>().UpdateAsync(product);
            await _unitOfWork.SaveAsync();

            if (product.QuantityInStock > 0 && oldQuantityInStock <= 0)
            {
                //Tự động xóa Pre-order khi số lượng sản phẩm > 0
                await UpdatePreOrdersDeletedTime(product.Id);
            }
            return product;
        }
    }
}
