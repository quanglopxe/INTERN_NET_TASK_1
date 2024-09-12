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
        [HttpGet]
        public async Task<IActionResult> GetProducts(string? id)
        {
            try
            {
                // Lấy tất cả sản phẩm
                var products = await _ProductsService.GetProducts(id);

                // Kiểm tra xem có sản phẩm nào không
                if (products == null || !products.Any())
                {
                    return NotFound("Không tìm thấy sản phẩm nào.");
                }

                return Ok(products); // Trả về danh sách sản phẩm
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ: {ex.Message}"); // Trả về mã lỗi 500
            }
        }
        [HttpPost()]
        public async Task<IActionResult> CreateProducts(ProductsModel ProductsModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseException.BadRequestException("BadRequest", ModelState.ToString()));
            }
            Products Products = await _ProductsService.CreateProducts(ProductsModel);
            return Ok(BaseResponse<Products>.OkResponse(Products));
        }
        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateProduct(string id, [FromBody] ProductsModel productsModel)
        //{
        //    if (id.Equals(productsModel.ProductId) == true)
        //    {
        //        return BadRequest("ID không khớp.");
        //    }

        //    try
        //    {
        //        var updatedProduct = await _ProductsService.UpdateProducts(productsModel);
        //        return Ok(updatedProduct); // Trả về sản phẩm đã cập nhật
        //    }
        //    catch (Exception ex)
        //    {
        //        return NotFound(ex.Message); // Trả về 404 nếu sản phẩm không tồn tại
        //    }
        //}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            try
            {
                var deletedProduct = await _ProductsService.DeleteProducts(id);
                return Ok(deletedProduct); // Trả về sản phẩm đã bị xóa
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message); // Trả về 404 nếu sản phẩm không tồn tại
            }
        }
    }
}
