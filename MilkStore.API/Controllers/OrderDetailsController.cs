using Microsoft.AspNetCore.Mvc;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.ModelViews.OrderDetailsModelView;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core.Base;
using Microsoft.AspNetCore.Authorization;
using MilkStore.ModelViews.ResponseDTO;
using MilkStore.Services.Service;

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
        public async Task<IActionResult> GetOrderDetails(string? orderId, int page = 1, int pageSize = 10)
        {
            IList<OrderDetails> detail = (IList<OrderDetails>)await _orderDetailsService.ReadOrderDetails(orderId, page, pageSize);
            return Ok(BaseResponse<IList<OrderDetails>>.OkResponse(detail));
        }

        // POST
        //[Authorize(Roles = "Guest, Member")]
        [HttpPost]
        public async Task<IActionResult> CreateOrderDetails(OrderDetailsModelView model)
        {           
            OrderDetailResponseDTO response = await _orderDetailsService.CreateOrderDetails(model);
            if(response.IsConfirmationRequired)
            {
                return Ok(new
                {
                    success = false,
                    message = response.Message,
                    orderDetails = response
                });
            }
            else
            {
                // Đơn hàng xử lý thành công
                return Ok(new
                {
                    success = true,
                    message = response.Message,
                    orderDetails = response
                });
            }
        }

        // Endpoint để xác nhận đơn hàng sau khi người dùng đồng ý pre-order
        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmOrderDetails(OrderDetailsConfirmationModel model)
        {
            OrderDetailResponseDTO response = await _orderDetailsService.ConfirmOrderDetails(model);

            return Ok(new
            {
                success = true,
                message = response.Message,
                orderDetails = response
            });                        
        }

        // PUT
        //[Authorize(Roles = "Guest, Member")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderDetails(string id, OrderDetailsModelView model)
        {            
            OrderDetails detail = await _orderDetailsService.UpdateOrderDetails(id, model);
            return Ok(BaseResponse<string>.OkResponse("Order detail update successfully!"));
        }

        // DELETE
        //[Authorize(Roles = "Guest, Member")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderDetails(string id)
        {            
            await _orderDetailsService.DeleteOrderDetails(id);
            return Ok(BaseResponse<string>.OkResponse("Order detail delete successfully!"));            
        }
    }
}

