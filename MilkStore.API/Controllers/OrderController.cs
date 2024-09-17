using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MilkStore.ModelViews.OrderModelViews;
using MilkStore.Contract.Services.Interface;
using MilkStore.Contract.Repositories.Entity;
using Microsoft.AspNetCore.Authorization;
using MilkStore.ModelViews.ResponseDTO;
using MilkStore.Services.Service;
using MilkStore.Core.Base;
using Microsoft.Extensions.Hosting;
using MilkStore.Core;

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
        
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll(string? id, int index = 1, int pageSize = 10)
        {
            IList<OrderResponseDTO> ord = (IList<OrderResponseDTO>)await _orderService.GetAsync(id);
            if(ord is null)
            {
                return NotFound(new { message = "Không tìm thấy Order" });
            }
            int totalItems = ord.Count;
            List<OrderResponseDTO> pagedOrder = ord.Skip((index - 1) * pageSize).Take(pageSize).ToList();

            // Tạo danh sách phân trang
            BasePaginatedList<OrderResponseDTO> paginatedList = new BasePaginatedList<OrderResponseDTO>(
                pagedOrder, totalItems, index, pageSize);
            return Ok(BaseResponse<BasePaginatedList<OrderResponseDTO>>.OkResponse(paginatedList));
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
            return Ok(new { message = "Add Order thành công" });
        }

        [HttpPut("{id}")]
        //[Authorize(Roles = "Guest, Member")]
        public async Task<IActionResult> Update(string id, OrderModelView item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseException.BadRequestException("BadRequest", ModelState.ToString()));
            }
            OrderResponseDTO existingOrder = (OrderResponseDTO) await _orderService.GetAsync(id);
            if (existingOrder is null)
            {
                return NotFound(new { message = "Không tìm thấy Order" });
            }
            Order ord = await _orderService.UpdateAsync(id, item);
            return Ok(new { message = "Update Order thành công" });
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Guest, Member")]
        public async Task<IActionResult> Delete(string id)
        {
            OrderResponseDTO existingOrder = (OrderResponseDTO)await _orderService.GetAsync(id);
            if (existingOrder is null)
            {
                return NotFound(new { message = "Không tìm thấy Order" });
            }
            await _orderService.DeleteAsync(id);
            return Ok(new { message = "Xóa thành công" });
        }
    }
}
