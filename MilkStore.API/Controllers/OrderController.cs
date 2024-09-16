using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MilkStore.ModelViews.OrderModelViews;
using MilkStore.Contract.Services.Interface;
using MilkStore.Contract.Repositories.Entity;
using Microsoft.AspNetCore.Authorization;
using MilkStore.ModelViews.ResponseDTO;
using MilkStore.Services.Service;
using MilkStore.Core.Base;

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
        public async Task<IActionResult> GetAll(string? id, int page = 1, int pageSize = 10)
        {
            IList<OrderResponseDTO> ord = (IList<OrderResponseDTO>)await _orderService.GetAsync(id);
            return Ok(BaseResponse<IList<OrderResponseDTO>>.OkResponse(ord));
        }


        [HttpPost]
        //[Authorize(Roles = "Guest, Member")]
        public async Task<IActionResult> Add(OrderModelView item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseException.BadRequestException("BadRequest", ModelState.ToString()));
            }
            Order ord = await _orderService.AddAsync(item);
            return Ok(BaseResponse<Order>.OkResponse(ord));
        }

        [HttpPut("{id}")]
        //[Authorize(Roles = "Guest, Member")]
        public async Task<IActionResult> Update(string id, OrderModelView item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseException.BadRequestException("BadRequest", ModelState.ToString()));
            }
            var items = await _orderService.GetAsync(id);
            if (items == null)
            {
                return NotFound();
            }
            Order ord = await _orderService.UpdateAsync(id, item);
            return Ok(BaseResponse<Order>.OkResponse(ord));
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
