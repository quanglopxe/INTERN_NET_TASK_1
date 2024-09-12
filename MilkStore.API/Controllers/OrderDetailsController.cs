using Microsoft.AspNetCore.Mvc;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.ModelViews;
using MilkStore.ModelViews.OrderDetailsModelView;
using MilkStore.Services.Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MilkStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailsController : ControllerBase
    {
        private readonly OrderDetailsService _orderDetailsService;
        public OrderDetailsController(OrderDetailsService orderDetailsService)
        {
            _orderDetailsService = orderDetailsService;
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> GetOrderDetails(Guid? orderId = null, Guid? productId = null, int page = 1, int pageSize = 10)
        {
            try
            {
                IEnumerable<OrderDetails> orderDetails;

                if (orderId.HasValue)
                {
                    if (productId.HasValue)
                    {
                        // Có cả orderId và productId
                        orderDetails = await _orderDetailsService.ReadOrderDetails(orderId.Value, productId.Value);
                        if (orderDetails == null || !orderDetails.Any())
                        {
                            return NotFound();
                        }
                    }
                    else
                    {
                        // Có orderId nhưng không có productId
                        orderDetails = await _orderDetailsService.ReadOrderDetails(orderId.Value, null, page, pageSize);
                    }
                }
                else if (productId.HasValue)
                {
                    // Không có orderId nhưng có productId
                    orderDetails = await _orderDetailsService.ReadOrderDetails(null, productId.Value, page, pageSize);
                }
                else
                {
                    // Không có orderId và không có productId
                    orderDetails = await _orderDetailsService.ReadOrderDetails(null, null, page, pageSize);
                }

                return Ok(orderDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> CreateOrderDetails(OrderDetailsModelView model)
        {
            try
            {
                await _orderDetailsService.CreateOrderDetails(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT
        [HttpPut]
        public async Task<IActionResult> UpdateOrderDetails(OrderDetailsModelView model)
        { 
            try
            {
                await _orderDetailsService.UpdateOrderDetails(model);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE
        [HttpDelete("{orderId}/{productId}")]
        public async Task<IActionResult> DeleteOrderDetails(Guid orderId, Guid productId, string deletedBy)
        {
            try
            {
                await _orderDetailsService.DeleteOrderDetails(orderId, productId, deletedBy);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
