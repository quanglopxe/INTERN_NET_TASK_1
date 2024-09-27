using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core.Base;
using MilkStore.ModelViews.ProductsModelViews;
using MilkStore.ModelViews.UserModelViews;
using MilkStore.Repositories.Entity;
using MilkStore.Services.Service;

namespace MilkStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _ProductsService;
        public ProductsController(IProductsService ProductsService)
        {
            _ProductsService = ProductsService;
        }
        [HttpGet("GetProduct & Pagging")]
        //[Authorize(Roles = "Admin,Member")]
        public async Task<IActionResult> GetProducts([FromQuery] string? id, [FromQuery] int pageIndex, [FromQuery] int pageSize)

        {
            
            var result = await _ProductsService.GetProducts(id, pageIndex, pageSize);
            return Ok(result);
            
        }
        [HttpGet("GetByName")]
        public async Task<IActionResult> GetByName(string? Productname, string? CategoryName)
        {
            IEnumerable<ProductsModel> product = await _ProductsService.GetProductsName(Productname, CategoryName);
            return Ok(product);
        }
        [HttpPost()]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProducts(ProductsModel ProductsModel)
        {
            await _ProductsService.CreateProducts(ProductsModel);
            return Ok(BaseResponse<string>.OkResponse("Added successfully"));
        }
        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(string id, [FromBody] ProductsModel productsModel)
        {
            await _ProductsService.UpdateProducts(id, productsModel);
            return Ok(BaseResponse<string>.OkResponse("Updated successfully"));
        }
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            await _ProductsService.DeleteProducts(id);
            return Ok(BaseResponse<string>.OkResponse("Deleted successfully"));
        }
    }
}
