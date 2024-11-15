using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core;
using MilkStore.Core.Base;
using MilkStore.ModelViews.OrderModelViews;
using MilkStore.ModelViews.ResponseDTO;
using MilkStore.Services.Service;

namespace MilkStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public CheckoutController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }
        [Authorize(Roles = "Member, Staff, Admin")]
        [HttpPost]
        public async Task<IActionResult> Checkout([FromQuery] PaymentMethod paymentMethod, [FromQuery] List<string>? voucherCode, [FromQuery] ShippingType shippingAddress)
        {
            string result = await _transactionService.Checkout(paymentMethod, voucherCode, shippingAddress);
            return Ok(BaseResponse<string>.OkResponse(result));
        }
        [Authorize(Roles = "Member, Staff, Admin")]
        [HttpPost("topup")]
        public async Task<IActionResult> Topup([FromQuery] double amount)
        {
            string result = await _transactionService.Topup(amount);
            return Ok(BaseResponse<string>.OkResponse(result));
        }
        [Authorize(Roles = "Member, Staff, Admin")]
        [HttpGet("personal_transaction_history")]
        public async Task<IActionResult> GetPersonalTransactionHistory([FromQuery] TransactionType? transactionType, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, [FromQuery] int? month, [FromQuery] int? year, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            BasePaginatedList<TransactionHistoryResponseDTO> result = await _transactionService.GetPersonalTransactionHistory(transactionType, fromDate, toDate, month, year, pageIndex, pageSize);
            return Ok(BaseResponse<BasePaginatedList<TransactionHistoryResponseDTO>>.OkResponse(result));
        }
        [Authorize(Roles = "Admin, Staff")]
        [HttpGet("all_transaction_history")]
        public async Task<IActionResult> GetAllTransactionHistoryAsync([FromQuery] string? userId, [FromQuery] TransactionType? transactionType, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, [FromQuery] int? month, [FromQuery] int? year, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            BasePaginatedList<TransactionHistoryResponseDTO> result = await _transactionService.GetAllTransactionHistoryAsync(userId, transactionType, fromDate, toDate, month, year, pageIndex, pageSize);
            return Ok(BaseResponse<BasePaginatedList<TransactionHistoryResponseDTO>>.OkResponse(result));
        }
    }
}
