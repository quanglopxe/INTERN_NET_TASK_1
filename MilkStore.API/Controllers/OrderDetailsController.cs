using Microsoft.AspNetCore.Mvc;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.ModelViews;
using MilkStore.ModelViews.OrderDetailsModelView;
using MilkStore.Services.Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MilkStore.Contract.Services.Interface;

namespace MilkStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailsController : ControllerBase
    {
        private readonly IOrderDetailsService _orderDetailsService;
        public OrderDetailsController(IOrderDetailsService orderDetailsService)
        {
            _orderDetailsService = orderDetailsService;
        }

        // GET
        [HttpGet]
        public async Task<IEnumerable<OrderDetails>> GetOrderDetails(string? orderId = null, int page = 1, int pageSize = 10)
        {
            return await _orderDetailsService.ReadOrderDetails(orderId, page, pageSize);
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
        public async Task<IActionResult> DeleteOrderDetails(string orderId, string productId)
        {
            try
            {
                await _orderDetailsService.DeleteOrderDetails(orderId, productId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
