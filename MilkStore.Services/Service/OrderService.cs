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

namespace MilkStore.Services.Service
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
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

        public async Task AddAsync(OrderModelView ord)
        {
            string? userID = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userID))
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, "Please log in first!");
            }
            // Sử dụng mapper để ánh xạ từ OrderModelView sang Order
            Order item = _mapper.Map<Order>(ord);
            item.UserId = Guid.Parse(userID);
            item.CreatedBy = userID;
            item.LastUpdatedBy = userID;
            item.OrderDate = CoreHelper.SystemTimeNow;
            DateTimeOffset d1 = item.OrderDate.AddDays(3);
            DateTimeOffset d2 = item.OrderDate.AddDays(5);
            item.estimatedDeliveryDate = $"từ {d1:dd/MM/yyyy} đến {d2:dd/MM/yyyy}";

            // Đảm bảo gán các giá trị khác không được ánh xạ từ model view
            item.TotalAmount = 0;
            item.DiscountedAmount = 0;

            await _unitOfWork.GetRepository<Order>().InsertAsync(item);
            await _unitOfWork.SaveAsync();                    
        }

        public async Task UpdateAsync(string id, OrderModelView ord)
        {
            string? userID = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (string.IsNullOrWhiteSpace(userID))
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, "Please log in!");
            }
            // Lấy đối tượng hiện tại từ cơ sở dữ liệu
            Order orderss = await _unitOfWork.GetRepository<Order>().Entities
                .FirstOrDefaultAsync(or => or.Id == id && !or.DeletedTime.HasValue)
                ?? throw new BaseException.ErrorException(Core.Constants.StatusCodes.NotFound, ErrorCode.NotFound, $"Order with ID  {id}  not found or has already been deleted."); 

            // Sử dụng AutoMapper để ánh xạ những thay đổi
            _mapper.Map(ord, orderss);  // Chỉ ánh xạ những thuộc tính có giá trị khác biệt


                

            // Cập nhật thời gian cập nhật
            orderss.LastUpdatedTime = CoreHelper.SystemTimeNow;

            // Lưu thay đổi vào cơ sở dữ liệu
            await _unitOfWork.GetRepository<Order>().UpdateAsync(orderss);
            await _unitOfWork.SaveAsync();                        
        }

        //Cập nhật TotalAmount
        public async Task UpdateToTalAmount(string id)
        {
            Order ord = await _unitOfWork.GetRepository<Order>().Entities
            .FirstOrDefaultAsync(or => or.Id == id && !or.DeletedTime.HasValue)
            ?? throw new BaseException.ErrorException(Core.Constants.StatusCodes.NotFound, ErrorCode.NotFound, $"Order with ID  {id}  not found or has already been deleted.");
            List<OrderDetails> lstOrd = await _unitOfWork.GetRepository<OrderDetails>().Entities
                .Where(ordt => ordt.OrderID == id && !ordt.DeletedTime.HasValue).ToListAsync();
            ord.TotalAmount = lstOrd.Sum(o => o.TotalAmount);

            double discountAmount = 0;
            //Tính thành tiền áp dụng ưu đãi
            Voucher? vch = await _unitOfWork.GetRepository<Voucher>().Entities
                .FirstOrDefaultAsync(v => v.Id == ord.VoucherId && !v.DeletedTime.HasValue);
            if (vch is
                {
                    ExpiryDate: DateTime expiryDate,
                    LimitSalePrice: int limitSalePrice,
                    SalePercent: int salePercent,
                    UsedCount: int usedCount,
                    UsingLimit: int usingLimit
                })
            {
                if (expiryDate > ord.OrderDate
                    && Convert.ToDouble(limitSalePrice) <= ord.TotalAmount
                    && usedCount < usingLimit)
                {
                    discountAmount = (ord.TotalAmount * salePercent) / 100.0;
                }
            }
            ord.DiscountedAmount = ord.TotalAmount - discountAmount;
            await _unitOfWork.GetRepository<Order>().UpdateAsync(ord);
            await _unitOfWork.SaveAsync();            
        }

        public async Task AddVoucher(string id, string voucherId)
        {
            Order orderss = await _unitOfWork.GetRepository<Order>().Entities
                .FirstOrDefaultAsync(or => or.Id == id && !or.DeletedTime.HasValue)
                ?? throw new BaseException.ErrorException(Core.Constants.StatusCodes.NotFound, ErrorCode.NotFound, $"Order with ID  {id}  not found or has already been deleted.");

            // Kiểm tra sự tồn tại của Voucher
            Voucher vch = await _unitOfWork.GetRepository<Voucher>().Entities
                    .FirstOrDefaultAsync(v => v.Id == voucherId && !v.DeletedTime.HasValue)
                    ?? throw new BaseException.ErrorException(Core.Constants.StatusCodes.NotFound, ErrorCode.NotFound, $"Voucher with ID {voucherId} not found."); 

            if (vch.ExpiryDate < orderss.OrderDate)
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, $"The voucher has expired.");
            }
            vch.UsedCount++;
            await _unitOfWork.GetRepository<Voucher>().UpdateAsync(vch);

            orderss.VoucherId = vch.Id;
            await _unitOfWork.GetRepository<Order>().UpdateAsync(orderss);
            await _unitOfWork.SaveAsync();
            await UpdateToTalAmount(orderss.Id);       
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


        public async Task GetStatus_Mail(string? id)
        {
            Order order = await _unitOfWork.GetRepository<Order>().GetByIdAsync(id);
            ApplicationUser? user = await _userManager.FindByIdAsync(order.UserId.ToString());
            if (order != null && order.DeletedTime == null)
            {
                if (order.UserId == user.Id)
                {
                    if (order.Status.Equals("successful payment") && order.PaymentMethod.Equals("Online"))
                    {
                        _emailService.SendEmailAsync(user.Email, "Đơn hàng " + order.TotalAmount + " thanh toán thành công", "Thời gian giao hàng dự kiến " + order.estimatedDeliveryDate + ". Cảm ơn quý khách đã mua hàng tại MilkStore");
                        order.Status = "successful payment - DONE";
                        await _unitOfWork.GetRepository<Order>().UpdateAsync(order);
                        await _unitOfWork.SaveAsync();
                    }
                    if (order.PaymentMethod.Equals("Offline"))
                    {
                        _emailService.SendEmailAsync(user.Email, "Đơn hàng " + order.TotalAmount + " thanh toán khi nhận hàng", "Thời gian giao hàng dự kiến " + order.estimatedDeliveryDate + ". Cảm ơn quý khách đã mua hàng tại MilkStore");
                        order.Status = "successful payment - DONE";
                        await _unitOfWork.GetRepository<Order>().UpdateAsync(order);
                        await _unitOfWork.SaveAsync();
                    }
                }
            }
        }
        public async Task GetNewStatus_Mail(string? id)
        {
            Order order = await _unitOfWork.GetRepository<Order>().GetByIdAsync(id);
            ApplicationUser? user = await _userManager.FindByIdAsync(order.UserId.ToString());
            string originalStatus = "waiting";
            if (order != null && order.DeletedTime == null)
            {
                if (order.UserId == user.Id)
                {
                    if (!string.IsNullOrEmpty(order.Status) && !order.Status.Equals(originalStatus))
                    {
                        string subject = "Đơn hàng của quý khách: " + order.User.UserName + " vừa được cập nhật";
                        string message = "Trạng thái đơn hàng: " + order.Id + " đã được thay đổi thành: " + order.Status + ". Cảm ơn quý khách đã mua hàng tại MilkStore.";

                        _emailService.SendEmailAsync(user.Email, subject, message);

                        await _unitOfWork.GetRepository<Order>().UpdateAsync(order);
                        await _unitOfWork.SaveAsync();
                    }
                }
            }
        }


        public async Task DeductStockOnDelivery(string orderId)
        {
            try
            {
                // Retrieve the order and its details
                Order order = await _unitOfWork.GetRepository<Order>().Entities
                    .FirstOrDefaultAsync(o => o.Id == orderId && !o.DeletedTime.HasValue)
                    ?? throw new KeyNotFoundException($"Order with ID {orderId} not found or deleted.");

                if (order.Status == "Confirmed")
                {
                    // Retrieve the order details
                    List<OrderDetails> orderDetailsList = await _unitOfWork.GetRepository<OrderDetails>().Entities
                        .Where(od => od.OrderID == order.Id).ToListAsync();

                    // Loop through the order details to deduct stock
                    foreach (var orderDetail in orderDetailsList)
                    {
                        Products product = await _unitOfWork.GetRepository<Products>().GetByIdAsync(orderDetail.ProductID);
                        if (product != null && product.QuantityInStock >= orderDetail.Quantity)
                        {
                            product.QuantityInStock -= orderDetail.Quantity; // Deduct quantity
                            await _unitOfWork.GetRepository<Products>().UpdateAsync(product);
                        }
                        else
                        {
                            throw new Exception($"Product with ID {orderDetail.ProductID} does not have enough stock.");
                        }
                    }

                    // Save changes
                    await _unitOfWork.SaveAsync();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error while deducting stock on delivery.", ex);
            }
        }
    }
}
