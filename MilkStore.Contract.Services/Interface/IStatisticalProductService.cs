using MilkStore.ModelViews.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.Contract.Services.Interface
{
    public interface IStatisticalProductService
    {
        Task<List<ProductRevenueDTO>> GetRevenueByProduct(DateTime? startDate, DateTime? endDate);
        Task<List<ProductSalesDTO>> GetBestWorstSellingProducts(DateTime? startDate, DateTime? endDate);
        Task<List<ProductStockDTO>> GetLowStockProducts(int threshold);
    }
}
