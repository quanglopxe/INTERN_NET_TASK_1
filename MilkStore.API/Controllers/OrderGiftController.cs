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

        [HttpPost()]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateOrderGift(OrderGiftModel OrderGiftModel)
        {

            await _OGiftService.CreateOrderGift(OrderGiftModel);
            return Ok(BaseResponse<string>.OkResponse("Added successfully"));
        }

        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderGift(string id, [FromBody] OrderGiftModel OrderGiftModel)
        {
            await _OGiftService.UpdateOrderGift(id, OrderGiftModel);
            return Ok(BaseResponse<string>.OkResponse("Updated successfully"));
            
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrderGift(string id)
        {
            await _OGiftService.DeleteOrderGift(id);
            return Ok(BaseResponse<string>.OkResponse("Deleted successfully"));
        }

    }
}
