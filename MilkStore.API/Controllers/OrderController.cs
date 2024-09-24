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
            try
            {
                IList<OrderResponseDTO> ord = (IList<OrderResponseDTO>)await _orderService.GetAsync(id);
                int totalItems = ord.Count;
                List<OrderResponseDTO> pagedOrder = ord.Skip((index - 1) * pageSize).Take(pageSize).ToList();

                // Tạo danh sách phân trang
                BasePaginatedList<OrderResponseDTO> paginatedList = new BasePaginatedList<OrderResponseDTO>(
                    pagedOrder, totalItems, index, pageSize);
                return Ok(BaseResponse<BasePaginatedList<OrderResponseDTO>>.OkResponse(paginatedList));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);  // Trả về 400 BadRequest nếu có lỗi do dữ liệu
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, "Lỗi máy chủ. Vui lòng thử lại sau.");  // Trả về 500 nếu có lỗi server
            }
        }
        //[HttpGet("SendMail")]
        //public async Task<IActionResult> SendMail(string ?id)
        //{
        //    IEnumerable<OrderModelView> order = new List<OrderModelView>();
        //    try
        //    {
        //        order = await _orderService.GetStatus_Mail(id);
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        return BadRequest("Lỗi !!!!");
        //    }
        //    return Ok(order);

        //}

        [HttpPost]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Add(OrderModelView item)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new BaseException.BadRequestException("BadRequest", ModelState.ToString()));
                }
                string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                await _orderService.AddAsync(item, userId);
                return Ok(BaseResponse<string>.OkResponse("Thêm thành công"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);  // Trả về 400 BadRequest nếu có lỗi do dữ liệu
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, "Lỗi máy chủ. Vui lòng thử lại sau.");  // Trả về 500 nếu có lỗi server
            }
        }

        [HttpPut("AddVoucher{id}")]
        //[Authorize(Roles = "Guest, Member")]
        public async Task<IActionResult> AddVoucher(string id, string item)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new BaseException.BadRequestException("BadRequest", ModelState.ToString()));
                }
                await _orderService.AddVoucher(id, item);
                return Ok(BaseResponse<string>.OkResponse("Add voucher thành công"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);  // Trả về 400 BadRequest nếu có lỗi do dữ liệu
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, "Lỗi máy chủ. Vui lòng thử lại sau.");  // Trả về 500 nếu có lỗi server
            }
        }


        [HttpPut("Update{id}")]
        //[Authorize(Roles = "Guest, Member")]
        public async Task<IActionResult> Update(string id, OrderModelView item)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new BaseException.BadRequestException("BadRequest", ModelState.ToString()));
                }
                await _orderService.UpdateAsync(id, item);
                await _orderService.GetStatus_Mail(id);
                await _orderService.GetNewStatus_Mail(id);
                return Ok(BaseResponse<string>.OkResponse("Update Order thành công"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);  // Trả về 400 BadRequest nếu có lỗi do dữ liệu
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, "Lỗi máy chủ. Vui lòng thử lại sau.");  // Trả về 500 nếu có lỗi server
            }
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Guest, Member")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _orderService.DeleteAsync(id);
                return Ok(new { message = "Xóa thành công" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);  // Trả về 400 BadRequest nếu có lỗi do dữ liệu
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, "Lỗi máy chủ. Vui lòng thử lại sau.");  // Trả về 500 nếu có lỗi server
            }
        }
    }
}
