﻿using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Repositories.Interface;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core.Base;
using MilkStore.Core.Constants;
using MilkStore.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.Services.Service
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TransactionService(IUnitOfWork unitOfWork, IServiceProvider serviceProvider, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _serviceProvider = serviceProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> Checkout(PaymentMethod paymentMethod, List<string>? voucherCode)
        {
            string userID = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new BaseException.ErrorException(Core.Constants.StatusCodes.Unauthorized, ErrorCode.Unauthorized, "User not found");
            var _paymentService = _serviceProvider.GetRequiredService<IPaymentService>();
            var _orderService = _serviceProvider.GetRequiredService<IOrderService>();

            // Kiểm tra xem có giỏ hàng hiện tại không
            var cartItems = await _unitOfWork.GetRepository<OrderDetails>().Entities
                .Where(od => od.CreatedBy == userID && od.Status == OrderDetailStatus.InCart && od.DeletedTime == null)
                .Include(od => od.Products) // Bao gồm thông tin sản phẩm để tính tổng giá trị
                .ToListAsync();

            if (!cartItems.Any())
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, "Your cart is empty. Please add items to cart before checkout.");
            }

            Order order = await _unitOfWork.GetRepository<Order>().Entities
                    .FirstOrDefaultAsync(o => o.CreatedBy == userID && o.OrderStatuss == OrderStatus.Pending && o.PaymentStatuss == PaymentStatus.Unpaid)
                    ?? throw new BaseException.ErrorException(Core.Constants.StatusCodes.NotFound, ErrorCode.NotFound, "Order not found");

            if (paymentMethod == PaymentMethod.Online)
            {
                List<string> orderIds = cartItems.Select(od => od.OrderID).ToList();
                if (!orderIds.Contains(order.Id))
                {
                    // Tạo đơn hàng
                    await _orderService.AddAsync(voucherCode, cartItems, paymentMethod);
                }                
                // Tính tổng giá trị đơn hàng               
                double totalAmount = order.DiscountedAmount;

                // Tạo mã hóa đơn duy nhất
                string invoiceCode = $"HD-{DateTime.Now:yyyyMMddHHmmssfff} {order.Id}";

                var paymentRequest = new PaymentRequest
                {
                    TotalAmount = totalAmount,
                    InvoiceCode = invoiceCode
                };

                string paymentUrl = _paymentService.CreatePayment(paymentRequest);

                return paymentUrl;

            }
            else
            {
                // Tạo đơn hàng
                await _orderService.AddAsync(voucherCode, cartItems, paymentMethod);
                return "Order has been created successfully!";
            }
        }
    }
}