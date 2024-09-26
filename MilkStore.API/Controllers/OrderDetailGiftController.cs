using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core.Base;
using MilkStore.ModelViews.OrderDetailGiftModelView;
using MilkStore.Services.Service;

namespace MilkStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderDetailGiftController : ControllerBase
    {
        private readonly IOrderDetailGiftService _orderDetailGiftService;

        public OrderDetailGiftController(IOrderDetailGiftService orderDetailGiftService)
        {
            _orderDetailGiftService = orderDetailGiftService;
        }

        // GET: api/OrderDetailGift
        [HttpGet]
        public async Task<IActionResult> GetOrderDetailGift([FromQuery] string? id)
        {
            var result = await _orderDetailGiftService.GetOrderDetailGift(id);
            if (result == null || !result.Any())
            {
                return NotFound("No gifts found.");
            }
            return Ok(result);
        }

        // POST: api/OrderDetailGift
        [HttpPost]
        public async Task<IActionResult> CreateOrderDetailGift([FromBody] OrderDetailGiftModel orderDetailGiftModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdGift = await _orderDetailGiftService.CreateOrderDetailGift(orderDetailGiftModel);

                return CreatedAtAction(nameof(GetOrderDetailGift), new { id = createdGift.Id }, createdGift);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/OrderDetailGift/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderDetailGift(string id, [FromBody] OrderDetailGiftModel orderDetailGiftModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedGift = await _orderDetailGiftService.UpdateOrderDetailGift(id, orderDetailGiftModel);
                return Ok(updatedGift);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // DELETE: api/OrderDetailGift/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderDetailGift(object id)
        {
            try
            {
                var deletedGift = await _orderDetailGiftService.DeleteOrderDetailGift(id);
                return Ok(deletedGift);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }

}
