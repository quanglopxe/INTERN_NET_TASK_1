using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MilkStore.ModelViews.OrderModelViews;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core.Model;

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

        [HttpGet]
        public async Task<IEnumerable<Order>> GetAll()
        {
            return await _orderService.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetById(Guid id)
        {
            var item = await _orderService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return item;
        }

        [HttpPost]
        public async Task<IActionResult> Add(OrderModelView item)
        {
            await _orderService.AddAsync(item);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, OrderModelView item)
        {
            var items = await _orderService.GetByIdAsync(id);
            if (items == null)
            {
                return NotFound();
            }
            await _orderService.UpdateAsync(id, item);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _orderService.DeleteAsync(id);
            return NoContent();
        }
    }
}
