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
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderDetails(string id, OrderDetailsModelView model)
        { 
            try
            {
                var items = await _orderDetailsService.ReadOrderDetails(id);
                if (items == null)
                {
                    return NotFound();
                }
                await _orderDetailsService.UpdateOrderDetails(id, model); 
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderDetails(string id)
        {
            try
            {
                await _orderDetailsService.DeleteOrderDetails(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
