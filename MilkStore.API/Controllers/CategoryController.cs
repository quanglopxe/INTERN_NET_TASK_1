using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core.Base;
using MilkStore.ModelViews.CategoryModelViews;
using MilkStore.Contract.Repositories.Entity;
using Microsoft.AspNetCore.Authorization;
using MilkStore.Core;
namespace MilkStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _CategoryService;
        public CategoryController(ICategoryService CategoryService)
        {
            _CategoryService = CategoryService;
        }
        [HttpGet]
        //[Authorize(Roles = "Admin,Member")]
        public async Task<IActionResult> GetCategory(string? id)
        {
            try
            {
                IEnumerable<CategoryModel>? Category = await _CategoryService.GetCategory(id);

                if (Category == null || !Category.Any())
                {
                    return NotFound(new BaseException.ErrorException(404, "NotFound", "Sản phẩm không tồn tại!!!"));
                }

                return Ok(); // Trả về danh sách 
            }
            catch (BaseException.ErrorException e)
            {
                // return StatusCode(500); // Trả về mã lỗi 500
                return StatusCode(e.StatusCode, new BaseException.ErrorException(e.StatusCode, e.ErrorDetail.ErrorCode, e.ErrorDetail.ErrorMessage.ToString()));
            }
        }
        [HttpGet("GetPagging")]
        //[Authorize(Roles = "Admin,Member")]
        public async Task<IActionResult> Paging(int index, int size)
        {
            BasePaginatedList<Category>? paging = await _CategoryService.PagingCategory(index, size);
            return Ok(paging);
        }
        [HttpPost()]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory(CategoryModel CategoryModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseException.BadRequestException("BadRequest", ModelState.ToString()));
            }
            try
            {
                Category Category = await _CategoryService.CreateCategory(CategoryModel);
                return Ok(BaseResponse<Category>.OkResponse(Category));
            }
            catch (BaseException.ErrorException e)
            {

                return StatusCode(e.StatusCode, new BaseException.ErrorException(e.StatusCode, e.ErrorDetail.ErrorCode, e.ErrorDetail.ErrorMessage.ToString()));
            }
        }
        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(string id, [FromBody] CategoryModel CategoryModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseException.BadRequestException("BadRequest", ModelState.ToString()));
            }
            try
            {
                Category? updatedProduct = await _CategoryService.UpdateCategory(id, CategoryModel);
                return Ok(updatedProduct);
            }
            catch (BaseException.ErrorException e)
            {
                return StatusCode(e.StatusCode, new BaseException.ErrorException(e.StatusCode, e.ErrorDetail.ErrorCode, e.ErrorDetail.ErrorMessage.ToString()));
            }
        }
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            try
            {
                var deletedProduct = await _CategoryService.DeleteCategory(id);
                return Ok(deletedProduct); // Trả về sản phẩm đã bị xóa
            }
            catch (Exception ex)
            {
                return NotFound("Sản phẩm không tồn tại!!!"); // Trả về 404 nếu sản phẩm không tồn tại
            }
        }
    }
}
