using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MilkStore.Contract.Services.Interface;
using MilkStore.Services.Service;

namespace MilkStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticalProductController : ControllerBase
    {
        private readonly IStatisticalProductService _statisticalProductService;
        public StatisticalProductController(IStatisticalProductService statisticalProduct)
        {
            _statisticalProductService = statisticalProduct;
        }
        [HttpGet("revenue-by-product")]
        public async Task<IActionResult> GetRevenueByProduct([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var result = await _statisticalProductService.GetRevenueByProduct(startDate, endDate);
            return result != null ? Ok(result) : NotFound();
        }
        [HttpGet("best-worst-selling-products")]
        public async Task<IActionResult> GetBestWorstSellingProducts([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var result = await _statisticalProductService.GetBestWorstSellingProducts(startDate, endDate);
            return result != null && result.Any() ? Ok(result) : NotFound();
        }
        [HttpGet("low-stock-products")]
        public async Task<IActionResult> GetLowStockProducts([FromQuery] int threshold)
        {
            var result = await _statisticalProductService.GetLowStockProducts(threshold);
            return result != null && result.Any() ? Ok(result) : NotFound();
        }
    }
}
