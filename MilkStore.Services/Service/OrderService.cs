using MilkStore.Contract.Services.Interface;
using MilkStore.ModelViews.OrderModelViews;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Repositories.Interface;
using MilkStore.Core.Utils;
using MilkStore.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using MilkStore.ModelViews.ResponseDTO;
using AutoMapper;
using MilkStore.Repositories.Entity;
using MilkStore.Services.EmailSettings;
using Microsoft.AspNetCore.Identity;
using MilkStore.Core;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using MilkStore.Core.Base;
using MilkStore.Core.Constants;
using PaymentStatus = MilkStore.Contract.Repositories.Entity.PaymentStatus;
using PaymentMethod = MilkStore.Contract.Repositories.Entity.PaymentMethod;
using OrderStatus = MilkStore.Contract.Repositories.Entity.OrderStatus;
using MilkStore.ModelViews.PreOrdersModelView;
using System;
using Microsoft.VisualBasic;
using MilkStore.ModelViews;
using ShippingType = MilkStore.ModelViews.OrderModelViews.ShippingType;

namespace MilkStore.Services.Service
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;                    
        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;                        
        }

        public async Task<BasePaginatedList<OrderResponseDTO>> GetAsync(string? id, int pageIndex, int pageSize)
        {
            IQueryable<Order>? query = _unitOfWork.GetRepository<Order>().Entities.Where(order => order.DeletedTime == null);
            if (!string.IsNullOrWhiteSpace(id))
            {
                query = query.Where(order => order.Id == id);
            }            
            BasePaginatedList<Order>? paginatedOrders = await _unitOfWork.GetRepository<Order>().GetPagging(query, pageIndex, pageSize);

            if (!paginatedOrders.Items.Any() && !string.IsNullOrWhiteSpace(id))
            {                
                Order? orderById = await query.FirstOrDefaultAsync();
                if (orderById != null)
                {
                    OrderResponseDTO? orderDto = _mapper.Map<OrderResponseDTO>(orderById);
                    return new BasePaginatedList<OrderResponseDTO>(new List<OrderResponseDTO> { orderDto }, 1, 1, 1);
                }                
            }
            //GetAll
            List<OrderResponseDTO>? orderDtosResult = _mapper.Map<List<OrderResponseDTO>>(paginatedOrders.Items);
            return new BasePaginatedList<OrderResponseDTO>(
                orderDtosResult,
                paginatedOrders.TotalItems,
                paginatedOrders.CurrentPage,
                paginatedOrders.PageSize
            );
        }

        public async Task AddAsync(List<string>? voucherCode, List<OrderDetails> orderItems, PaymentMethod paymentMethod, ShippingType shippingAddress)
        {
            string userID = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userID))
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.Unauthorized, ErrorCode.Unauthorized, "Please log in first!");
            }
            var user = await _userManager.FindByIdAsync(userID) ?? throw new BaseException.ErrorException(Core.Constants.StatusCodes.NotFound, ErrorCode.NotFound, "User not found");
            if (string.IsNullOrWhiteSpace(user.ShippingAddress))
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, "Please update your shipping address before checkout!");
            }
            string shipMethod = shippingAddress.ToString();
            if(shippingAddress == ShippingType.InStore)
            {
                shipMethod = "Milk Store";
            }
            else
            {
                shipMethod = user.ShippingAddress;
            }
            Order order = new Order
            {
                UserId = Guid.Parse(userID),
                CreatedBy = userID,
                OrderDate = CoreHelper.SystemTimeNow,
                estimatedDeliveryDate = $"từ {CoreHelper.SystemTimeNow.AddDays(3):dd/MM/yyyy} đến {CoreHelper.SystemTimeNow.AddDays(5):dd/MM/yyyy}",
                
                ShippingAddress = shipMethod,
                TotalAmount = 0,
                DiscountedAmount = 0,
                PaymentStatuss = PaymentStatus.Unpaid,
                OrderStatuss = OrderStatus.Pending,
                PaymentMethod = paymentMethod
            };


            // Kiểm tra các mặt hàng mà người dùng mua
            if (orderItems == null || !orderItems.Any())
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, "Product list cannot be empty.");
            }
            // Kiểm tra giới hạn số lượng voucher (tối đa 3)
            if (voucherCode is not null && voucherCode.Count > 3)
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, "You can apply a maximum of 3 vouchers to an order.");
            }

            // Tính tổng tiền dựa trên danh sách sản phẩm từ orderItems
            order.TotalAmount = orderItems.Sum(o => o.TotalAmount);

            double totalDiscount = 0;
            double discountedTotal = order.TotalAmount;
            List<string> invalidVouchers = new List<string>();
            // Xử lý danh sách voucher nếu có
            if (voucherCode is not null && voucherCode.Any())
            {
                foreach (var voucherId in voucherCode)
                {
                    Voucher vch = await _unitOfWork.GetRepository<Voucher>().Entities
                        .FirstOrDefaultAsync(v => v.Id == voucherId && !v.DeletedTime.HasValue)
                        ?? throw new BaseException.ErrorException(Core.Constants.StatusCodes.NotFound, ErrorCode.NotFound, $"Voucher with ID {voucherId} not found.");

                    if (vch.ExpiryDate < order.OrderDate)
                    {
                        throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, $"The voucher with ID {voucherId} has expired.");
                    }

                    // Kiểm tra điều kiện áp dụng voucher
                    if (discountedTotal < Convert.ToDouble(vch.LimitSalePrice))
                    {
                        throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, $"The total amount does not meet the requirements to apply voucher {voucherId}.");
                    }
                    // Tính toán giảm giá nếu voucher hợp lệ
                    double discountAmount = (discountedTotal * vch.SalePercent) / 100.0;
                    discountedTotal -= discountAmount;
                    totalDiscount += discountAmount;

                    // Tạo bản ghi trong bảng OrderVoucher
                    var orderVoucher = new OrderVoucher
                    {
                        OrderId = order.Id,
                        VoucherId = vch.Id
                    };
                    await _unitOfWork.GetRepository<OrderVoucher>().InsertAsync(orderVoucher);

                    // Cập nhật số lần sử dụng voucher
                    vch.UsedCount++;
                    await _unitOfWork.GetRepository<Voucher>().UpdateAsync(vch);
                }
            }
            // Nếu có voucher không hợp lệ, ném exception với danh sách thông báo
            if (invalidVouchers.Any())
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, string.Join(", ", invalidVouchers));
            }

            await _unitOfWork.GetRepository<Order>().InsertAsync(order);
            await _unitOfWork.SaveAsync();

            // Gán OrderID cho mỗi OrderDetail và lưu chúng vào cơ sở dữ liệu
            orderItems.ForEach(item => item.OrderID = order.Id);
            await _unitOfWork.GetRepository<OrderDetails>().UpdateRangeAsync(orderItems);

            // Cập nhật lại tổng tiền sau khi đã áp dụng voucher
            await UpdateToTalAmount(order.Id);
        }

        public async Task UpdateOrder(string id, OrderModelView ord, OrderStatus orderStatus, PaymentStatus paymentStatus, PaymentMethod paymentMethod)
        {
            await UpdateAsync(id, ord, orderStatus, paymentStatus, paymentMethod);
            await UpdateInventoryQuantity(id);
            await UpdateUserPoint(id);
            await SendingPaymentStatus_Mail(id);
            await SendingOrderStatus_Mail(id);
        }
        public async Task UpdateAsync(string id, OrderModelView ord, OrderStatus orderStatus, PaymentStatus paymentStatus, PaymentMethod paymentMethod)
        {
            string? userID = _httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userID))
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.Unauthorized, ErrorCode.Unauthorized, "Hãy đăng nhập trước!");
            }
            // Lấy đối tượng hiện tại từ cơ sở dữ liệu
            Order orderss = await _unitOfWork.GetRepository<Order>().Entities
                .FirstOrDefaultAsync(or => or.Id == id && !or.DeletedTime.HasValue)
                ?? throw new BaseException.ErrorException(Core.Constants.StatusCodes.NotFound, ErrorCode.NotFound, $"Không tìm thấy đơn hàng có ID {id} hoặc đã bị xóa."); 

            // Sử dụng AutoMapper để ánh xạ những thay đổi
            _mapper.Map(ord, orderss);  // Chỉ ánh xạ những thuộc tính có giá trị khác biệt

            // Cập nhật trạng thái đơn hàng
            orderss.OrderStatuss = orderStatus;
            orderss.PaymentStatuss = paymentStatus;
            orderss.PaymentMethod = paymentMethod;

            // Cập nhật thời gian cập nhật
            orderss.LastUpdatedTime = CoreHelper.SystemTimeNow;
            orderss.LastUpdatedBy = userID;

            // Lưu thay đổi vào cơ sở dữ liệu
            await _unitOfWork.GetRepository<Order>().UpdateAsync(orderss);
            await _unitOfWork.SaveAsync();                
            
        }
        public async Task UpdateUserPoint(string id)
        {
            if(string.IsNullOrWhiteSpace(id))
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.NotFound, ErrorCode.NotFound, "Order ID cannot be null");
            }
            Order? order = await _unitOfWork.GetRepository<Order>().GetByIdAsync(id)?? throw new BaseException.ErrorException(Core.Constants.StatusCodes.NotFound, ErrorCode.NotFound, "Order not found");
            ApplicationUser? user = await _userManager.FindByIdAsync(order.UserId.ToString())?? throw new BaseException.ErrorException(Core.Constants.StatusCodes.NotFound, ErrorCode.NotFound, "User not found");
            if(order.OrderStatuss == OrderStatus.Delivered && order.PaymentStatuss == PaymentStatus.Paid && !order.IsPointAdded)
            {
                await _userService.AccumulatePoints(user.Id.ToString(), order.TotalAmount);
                order.PointsAdded = (int)(order.TotalAmount / 10000) * 10;
                order.IsPointAdded = true;
                await _unitOfWork.GetRepository<Order>().UpdateAsync(order);
                await _unitOfWork.SaveAsync();
            }
        }
        //Cập nhật TotalAmount
        public async Task UpdateToTalAmount(string id)
        {
            Order ord = await _unitOfWork.GetRepository<Order>().Entities
                .FirstOrDefaultAsync(or => or.Id == id && !or.DeletedTime.HasValue)
                ?? throw new BaseException.ErrorException(Core.Constants.StatusCodes.NotFound, ErrorCode.NotFound, $"Order with ID {id} not found or has already been deleted.");

            List<OrderDetails> lstOrd = await _unitOfWork.GetRepository<OrderDetails>().Entities
                .Where(ordt => ordt.OrderID == id && !ordt.DeletedTime.HasValue).ToListAsync();

            ord.TotalAmount = lstOrd.Sum(o => o.TotalAmount);

            double totalDiscount = 0;
            double discountedTotal = ord.TotalAmount;

            // Lấy danh sách các voucher đã áp dụng
            List<OrderVoucher> orderVouchers = await _unitOfWork.GetRepository<OrderVoucher>().Entities
                .Where(ov => ov.OrderId == ord.Id)
                .ToListAsync();

            foreach (var orderVoucher in orderVouchers)
            {
                Voucher? vch = await _unitOfWork.GetRepository<Voucher>().Entities
                    .FirstOrDefaultAsync(v => v.Id == orderVoucher.VoucherId && !v.DeletedTime.HasValue);

                if (vch is
                    {
                        ExpiryDate: DateTime expiryDate,
                        LimitSalePrice: int limitSalePrice,
                        SalePercent: int salePercent,
                        UsedCount: int usedCount,
                        UsingLimit: int usingLimit
                    })
                {                    
                    // Tính toán giảm giá nếu voucher hợp lệ
                    double discountAmount = (discountedTotal * salePercent) / 100.0;
                    discountedTotal -= discountAmount;
                    totalDiscount += discountAmount;
                }
            }
            
            // Cập nhật tổng số tiền sau khi giảm giá
            ord.DiscountedAmount = ord.TotalAmount - totalDiscount;

            // Cập nhật lại thông tin đơn hàng trong cơ sở dữ liệu
            await _unitOfWork.GetRepository<Order>().UpdateAsync(ord);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(string id)
        {
            Order orderss = await _unitOfWork.GetRepository<Order>().Entities
                .FirstOrDefaultAsync(or => or.Id == id && !or.DeletedTime.HasValue)
                ?? throw new BaseException.ErrorException(Core.Constants.StatusCodes.NotFound, ErrorCode.NotFound, $"Order with ID  {id}  not found or has already been deleted.");

            // Kiểm tra xem Order này có bất kỳ OrderDetails nào liên kết không
            bool hasOrderDetails = await _unitOfWork.GetRepository<OrderDetails>().Entities
                .AnyAsync(od => od.OrderID == id);

            if (hasOrderDetails)
            {
                // Trả về thông báo lỗi nếu tồn tại OrderDetails
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, $"Order with ID {id} is linked to OrderDetails and cannot be deleted.");
            }

            orderss.DeletedTime = CoreHelper.SystemTimeNow;
            await _unitOfWork.GetRepository<Order>().UpdateAsync(orderss);
            await _unitOfWork.SaveAsync();                        
        }


        public async Task SendingPaymentStatus_Mail(string id)
        {
            if(string.IsNullOrWhiteSpace(id))
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.NotFound, ErrorCode.NotFound, "Order ID cannot be null");
            }
            Order? order = await _unitOfWork.GetRepository<Order>().GetByIdAsync(id);
            if (order == null || order.DeletedTime != null)
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.NotFound, ErrorCode.NotFound, "Order not found");
            }
            ApplicationUser? user = await _userManager.FindByIdAsync(order.UserId.ToString());
            if (user == null)
            {
                   throw new BaseException.ErrorException(Core.Constants.StatusCodes.NotFound, ErrorCode.NotFound, "User not found");
            }

            if (order.PaymentMethod == PaymentMethod.Online && order.PaymentStatuss == PaymentStatus.Paid && order.OrderStatuss == OrderStatus.Confirmed)
            {
                await _emailService.SendEmailAsync(user.Email, "Đơn hàng " + order.Id + " đã thanh toán thành công.", "Thời gian giao hàng dự kiến " + order.estimatedDeliveryDate + ". Cảm ơn quý khách đã mua hàng tại MilkStore");                        
            }
            if (order.PaymentMethod == PaymentMethod.COD && order.PaymentStatuss == PaymentStatus.Unpaid)
            {
                if(order.OrderStatuss == OrderStatus.Confirmed)
                {
                    await _emailService.SendEmailAsync(user.Email, "Đơn hàng " + order.Id + " đã được xác nhận.", "Thời gian giao hàng dự kiến " + order.estimatedDeliveryDate + ". Quý khách vui lòng thanh toán số tiền: " + order.TotalAmount + "VNĐ khi nhận hàng. Cảm ơn quý khách đã mua hàng tại MilkStore!");
                }
            }
                          
        }
        public async Task SendingOrderStatus_Mail(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.NotFound, ErrorCode.NotFound, "Order ID cannot be null");
            }
            Order? order = await _unitOfWork.GetRepository<Order>().GetByIdAsync(id);
            if (order == null || order.DeletedTime != null)
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.NotFound, ErrorCode.NotFound, "Order not found");
            }
            ApplicationUser? user = await _userManager.FindByIdAsync(order.UserId.ToString());
            if (user == null)
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.NotFound, ErrorCode.NotFound, "User not found");
            }
            if (order.OrderStatuss != OrderStatus.Pending)
            {
                string subject = "Đơn hàng của quý khách: " + order.User.UserName + " vừa được cập nhật";
                string message = "Trạng thái đơn hàng: " + order.Id + " đã được thay đổi thành: " + order.OrderStatuss + ". Cảm ơn quý khách đã mua hàng tại MilkStore.";

                await _emailService.SendEmailAsync(user.Email, subject, message);                
            }               
        }


        public async Task UpdateInventoryQuantity(string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, "Order ID cannot be null");
            }
            
            Order order = await _unitOfWork.GetRepository<Order>().Entities
                .FirstOrDefaultAsync(o => o.Id == orderId && !o.DeletedTime.HasValue)
                ?? throw new BaseException.ErrorException(Core.Constants.StatusCodes.NotFound, ErrorCode.NotFound, "Order not found");

            if (order.OrderStatuss == OrderStatus.Confirmed && !order.IsInventoryUpdated)
            {                
                List<OrderDetails> orderDetailsList = await _unitOfWork.GetRepository<OrderDetails>().Entities
                    .Where(od => od.OrderID == order.Id).ToListAsync();

                // Loop through the order details to deduct stock
                foreach (var orderDetail in orderDetailsList)
                {
                    Products? product = await _unitOfWork.GetRepository<Products>().GetByIdAsync(orderDetail.ProductID);
                    if(product == null)
                    {
                        throw new BaseException.ErrorException(Core.Constants.StatusCodes.NotFound, ErrorCode.NotFound, $"Product with ID {orderDetail.ProductID} not found");
                    }
                    if (product.QuantityInStock >= orderDetail.Quantity)
                    {
                        product.QuantityInStock -= orderDetail.Quantity; // Deduct quantity                        
                        await _unitOfWork.GetRepository<Products>().UpdateAsync(product);

                        order.IsInventoryUpdated = true;
                        await _unitOfWork.GetRepository<Order>().UpdateAsync(order);

                        await _unitOfWork.SaveAsync();
                    }
                    else
                    {
                        throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, $"Not enough stock for product {product.ProductName}");
                    }
                }

                // Save changes
                await _unitOfWork.SaveAsync();
            }
        }
    }
}
