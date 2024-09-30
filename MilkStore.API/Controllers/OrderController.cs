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
using MilkStore.Repositories.Entity;
using MilkStore.Core.Utils;
using System.Security.Claims;

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
            BasePaginatedList<OrderResponseDTO> ord = await _orderService.GetAsync(id,index,pageSize);
            return  Ok(BaseResponse<BasePaginatedList<OrderResponseDTO>>.OkResponse(ord));
        }
        

        [HttpPost]
        //[Authorize(Roles = "Member")]
        public async Task<IActionResult> Add(OrderModelView item)
        {            
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _orderService.AddAsync(item, userId);
            return Ok(BaseResponse<string>.OkResponse("Order added successfully!"));            
        }

        [HttpPut("AddVoucher{id}")]
        //[Authorize(Roles = "Guest, Member")]
        public async Task<IActionResult> AddVoucher(string id, string item)
        {            
            await _orderService.AddVoucher(id, item);
            return Ok(BaseResponse<string>.OkResponse("Voucher added successfully!"));           
        }


        [HttpPut("Update{id}")]
        //[Authorize(Roles = "Guest, Member")]
        public async Task<IActionResult> Update(string id, OrderModelView item)
        {            
            await _orderService.UpdateAsync(id, item);
            await _orderService.GetStatus_Mail(id);
            await _orderService.GetNewStatus_Mail(id);
            return Ok(BaseResponse<string>.OkResponse("Order update successfully!"));           
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Guest, Member")]
        public async Task<IActionResult> Delete(string id)
        {            
            await _orderService.DeleteAsync(id);
            return Ok(new { message = "Order delete successfully!" });
            
        }

        [HttpPut("UpdateQuantity{id}")]
        //[Authorize(Roles = "Guest, Member")]
        public async Task<IActionResult> UpdateQuantity(string id, OrderModelView item)
        {                
            // Call the regular update method
            await _orderService.UpdateAsync(id, item);
            // If status is "Confirmed", deduct stock
            if (item.Status == "Confirmed")
            {
                await _orderService.DeductStockOnDelivery(id);
            }
            return Ok(new { message = "Update Order thành công" });       
        }

    }
}
