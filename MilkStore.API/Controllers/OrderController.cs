using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MilkStore.ModelViews.OrderModelViews;
using MilkStore.Contract.Services.Interface;
using MilkStore.Contract.Repositories.Entity;
using Microsoft.AspNetCore.Authorization;

namespace MilkStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet()]
        [Authorize(Roles = "Admin")]
        public async Task<IEnumerable<Order>> GetAll(string? id)
        {
            return await _orderService.GetAsync(id);
        }


        [HttpPost]
        //[Authorize(Roles = "Guest, Member")]
        public async Task<IActionResult> Add(OrderModelView item)
        {
            await _orderService.AddAsync(item);
            return Ok();
        }

        [HttpPut("{id}")]
        //[Authorize(Roles = "Guest, Member")]
        public async Task<IActionResult> Update(string id, OrderModelView item)
        {
            var items = await _orderService.GetAsync(id);
            if (items == null)
            {
                return NotFound();
            }
            await _orderService.UpdateAsync(id, item);
            return NoContent();
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Guest, Member")]
        public async Task<IActionResult> Delete(string id)
        {
            await _orderService.DeleteAsync(id);
            return NoContent();
        }
    }
}
