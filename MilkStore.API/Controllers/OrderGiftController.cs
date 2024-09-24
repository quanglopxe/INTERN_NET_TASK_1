using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core.Base;
using MilkStore.Core;
using MilkStore.ModelViews.OrderGiftModelViews;

namespace MilkStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderGiftController : ControllerBase
    {
        private readonly IOrderGiftService _OGiftService;
        public OrderGiftController(IOrderGiftService GiftService)
        {
            _OGiftService = GiftService;
        }

        [HttpGet]
        //[Authorize(Roles = "Admin,Member")]
        public async Task<IActionResult> GetOrderGift(string? id)
        {
            try
            {
                IEnumerable<OrderGiftModel> Gift = await _OGiftService.GetOrderGift(id);

                if (Gift == null || !Gift.Any())
                {
                    return NotFound("Quà tặng không tồn tại!!!");
                }

                return Ok(Gift);
            }
            catch (Exception ex)
            {
                return StatusCode(500); // Trả về mã lỗi 500
            }
        }



        //[HttpGet("GetPagging")]
        ////[Authorize(Roles = "Admin,Member")]
        //public async Task<IActionResult> Paging(int index, int size)
        //{
        //    BasePaginatedList<Gift> paging = await _OGiftService.PagingGift(index, size);
        //    return Ok(paging);
        //}

        [HttpPost()]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateOrderGift(OrderGiftModel OrderGiftModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseException.BadRequestException("BadRequest", ModelState.ToString()));
            }

            OrderGift Gift = await _OGiftService.CreateOrderGift(OrderGiftModel);
            return Ok(BaseResponse<OrderGift>.OkResponse(Gift));
        }

        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderGift(string id, [FromBody] OrderGiftModel OrderGiftModel)
        {
            if (!ModelState.IsValid)
            {
                return NotFound("Sản phẩm không tồn tại!!!");
            }

            try
            {
                OrderGift updatedProduct = await _OGiftService.UpdateOrderGift(id, OrderGiftModel);
                await _OGiftService.SendMail_OrderGift(id);
                return Ok(updatedProduct);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrderGift(string id)
        {
            try
            {
                OrderGift deletedProduct = await _OGiftService.DeleteOrderGift(id);
                return Ok(deletedProduct); // Trả về sản phẩm đã bị xóa
            }
            catch (Exception ex)
            {
                return NotFound("Sản phẩm không tồn tại!!!"); // Trả về 404 nếu sản phẩm không tồn tại
            }
        }
    }
}
