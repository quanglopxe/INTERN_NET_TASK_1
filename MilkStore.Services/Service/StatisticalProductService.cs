using Microsoft.EntityFrameworkCore;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Repositories.Interface;
using MilkStore.Contract.Services.Interface;
using MilkStore.ModelViews.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.Services.Service
{
    public class StatisticalProductService : IStatisticalProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        public StatisticalProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<ProductRevenueDTO>> GetRevenueByProduct(DateTime? startDate, DateTime? endDate)
        {
            var query = from od in _unitOfWork.GetRepository<OrderDetails>().Entities
                        join o in _unitOfWork.GetRepository<Order>().Entities on od.OrderID equals o.Id
                        where o.OrderStatuss == OrderStatus.Delivered
                        select new { od.ProductID, od.Quantity, od.UnitPrice, o.OrderDate };

            if (startDate.HasValue)
            {
                query = query.Where(x => x.OrderDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(x => x.OrderDate <= endDate.Value);
            }

            var revenueByProduct = await query
                .GroupBy(x => x.ProductID)
                .Select(g => new ProductRevenueDTO
                {
                    ProductId = g.Key,
                    TotalRevenue = g.Sum(x => x.Quantity * x.UnitPrice),
                    TotalQuantity = g.Sum(x => x.Quantity)
                })
                .ToListAsync();

            return revenueByProduct;
        }
        public async Task<List<ProductSalesDTO>> GetBestWorstSellingProducts(DateTime? startDate, DateTime? endDate)
        {
            var query = from od in _unitOfWork.GetRepository<OrderDetails>().Entities
                        join o in _unitOfWork.GetRepository<Order>().Entities on od.OrderID equals o.Id
                        where o.OrderStatuss == OrderStatus.Delivered
                        select new { od.ProductID, od.Quantity, o.OrderDate };

            if (startDate.HasValue)
            {
                query = query.Where(x => x.OrderDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(x => x.OrderDate <= endDate.Value);
            }

            var salesByProduct = await query
                .GroupBy(x => x.ProductID)
                .Select(g => new ProductSalesDTO
                {
                    ProductId = g.Key,
                    TotalSold = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(p => p.TotalSold) 
                .ToListAsync();

            return salesByProduct;
        }
        public async Task<List<ProductStockDTO>> GetLowStockProducts(int threshold)
        {
            var lowStockProducts = await _unitOfWork.GetRepository<Products>().Entities
                .Where(p => p.QuantityInStock <= threshold) 
                .Select(p => new ProductStockDTO
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    QuantityInStock = p.QuantityInStock,
                    CategoryId = p.CategoryId
                })
                .ToListAsync();

            return lowStockProducts;
        }
    }
}
