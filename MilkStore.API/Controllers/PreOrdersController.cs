using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core.Base;
using MilkStore.ModelViews.PreOrdersModelView;

namespace MilkStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreOrdersController : ControllerBase
    {
        private readonly IPreOrdersService _preOrdersService;
        public PreOrdersController(IPreOrdersService preOrdersService)
        {
            _preOrdersService = preOrdersService;
        }
        [HttpGet]
        [Authorize(Roles = "Admin, Member")]
        public async Task<IActionResult> GetPreOrders(string? id)
        {
            try
            {
                var preords = await _preOrdersService.GetPreOrders(id);

                if (preords == null || !preords.Any())
                {
                    return NotFound("Pre-order không tồn tại!!!");
                }

                return Ok(preords);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }
        [HttpPost()]
        [Authorize(Roles = "Admin, Member")]
        public async Task<IActionResult> CreatePreOrders(PreOrdersModelView preOrdersModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseException.BadRequestException("BadRequest", ModelState.ToString()));
            }
            PreOrders PreOrder = await _preOrdersService.CreatePreOrders(preOrdersModel);
            return Ok(BaseResponse<PreOrders>.OkResponse(PreOrder));
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePreOrder(string id, [FromBody] PreOrdersModelView preOrdersModel)
        {
            if (!ModelState.IsValid)
            {
                return NotFound("Pre-order không tồn tại!!!");
            }

            try
            {
                var updatedPreOrder = await _preOrdersService.UpdatePreOrders(id, preOrdersModel);
                return Ok(updatedPreOrder);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Member")]
        public async Task<IActionResult> DeletePreOrder(string id)
        {
            try
            {
                var deletedPreOrder = await _preOrdersService.DeletePreOrders(id);
                return Ok(deletedPreOrder);
            }
            catch (Exception ex)
            {
                return NotFound("Pre-order không tồn tại!!!");
            }
        }
        [HttpGet("Pagination")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Pagination([FromQuery] int pageSize, [FromQuery] int pageNumber)
        {
            var result = await _preOrdersService.Pagination(pageSize, pageNumber);
            return Ok(result);
        }
    }
}
