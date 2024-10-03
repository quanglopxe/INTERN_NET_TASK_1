using MilkStore.ModelViews.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.Contract.Services.Interface
{
    public interface IStatisticalService
    {
        Task<double> GetRevenueStats(DateTime? startDate, DateTime? endDate);
        Task<List<EmployeeRevenueDTO>> GetRevenueByEmployee(DateTime? startDate, DateTime? endDate);
        Task<List<ProductRevenueDTO>> GetRevenueByProduct(DateTime? startDate, DateTime? endDate);
        Task<List<CategoryRevenueDTO>> GetRevenueByCategory(DateTime? startDate, DateTime? endDate);

    }
}
