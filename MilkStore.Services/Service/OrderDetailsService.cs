using Microsoft.EntityFrameworkCore;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Repositories.Interface;
using MilkStore.ModelViews.OrderDetailsModelView;
using MilkStore.Repositories.Context;
using MilkStore.Core.Utils;
using MilkStore.Contract.Services.Interface;
using AutoMapper;
using MilkStore.ModelViews.ResponseDTO;
using MilkStore.Core.Base;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using MilkStore.ModelViews.PreOrdersModelView;
using MilkStore.Core.Constants;
using PreOrderStatus = MilkStore.ModelViews.PreOrdersModelView.PreOrderStatus;

namespace MilkStore.Services.Service
{
    public class OrderDetailsService : IOrderDetailsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderService _orderService;
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPreOrdersService _preOrdersService;

        public OrderDetailsService(IUnitOfWork unitOfWork, DatabaseContext context, IOrderService orderService, IMapper mapper, IHttpContextAccessor httpContextAccessor, IPreOrdersService preOrdersService)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _orderService = orderService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _preOrdersService = preOrdersService;
        }
        //private OrderDetailResponseDTO MapToOrderDetailResponseDTO(OrderDetails details)
        //{
        //    return _mapper.Map<OrderDetailResponseDTO>(details);
        //}

        // Create OrderDetails
        public async Task<OrderDetails> CreateOrderDetails(OrderDetailsModelView model)
        {
            try
            {
                string? userID = _httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier).Value;
                if (userID == null)
                {
                    throw new BaseException.ErrorException(Core.Constants.StatusCodes.Unauthorized, ErrorCode.Unauthorized, "Please log in first!");
                }
                // Kiểm tra xem số lượng có hợp lệ không
                if (model.Quantity <= 0 || model.Quantity % 1 != 0)
                {
                    throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, "Quantity must be greater than 0 and an integer.");
                }

                // Truy cập trực tiếp vào DbContext để tìm sản phẩm
                Products product = await _context.Products.FirstOrDefaultAsync(p => p.Id == model.ProductID);
                if (product == null)
                {
                    throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, $"Product with {product} not found!");
                }
                //PreOrder
                if (product.QuantityInStock < model.Quantity)
                {
                    PreOrdersModelView preOrdersModelView = new PreOrdersModelView
                    {
                        ProductID = model.ProductID,
                        Quantity = model.Quantity,
                        Status = PreOrderStatus.Pending,
                        UserID = Guid.Parse(userID)
                    };
                    await _preOrdersService.CreatePreOrders(preOrdersModelView);
                    throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, $"Product {product.ProductName} does not have sufficient quantity. Please check your email for more information!");
                }
                // Kiểm tra xem OrderDetails đã tồn tại hay chưa dựa trên OrderID và ProductID
                OrderDetails? existingOrderDetail = await _unitOfWork.GetRepository<OrderDetails>().Entities
                    .FirstOrDefaultAsync(od => od.OrderID == model.OrderID && od.ProductID == model.ProductID && od.DeletedTime == null);

                if (existingOrderDetail != null)
                {
                    // Nếu đã tồn tại, cập nhật số lượng và tính lại tổng tiền
                    existingOrderDetail.Quantity += model.Quantity;
                    existingOrderDetail.UnitPrice = product.Price;
                    existingOrderDetail.CreatedBy = userID;                    
                }
                else
                {
                    // Nếu chưa tồn tại, tạo OrderDetails mới
                    OrderDetails orderDetails = _mapper.Map<OrderDetails>(model);
                    orderDetails.UnitPrice = product.Price;

                    // Thêm mới OrderDetails
                    _context.OrderDetails.Add(orderDetails);
                    existingOrderDetail = orderDetails;
                }
                await _context.SaveChangesAsync();

                // Cập nhật tổng giá trị đơn hàng
                await _orderService.UpdateToTalAmount(model.OrderID);
                return existingOrderDetail;
            }
            catch (Exception ex)
            {
                string innerExceptionMessage = ex.InnerException?.Message ?? ex.Message;
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, $"An error occurred: {innerExceptionMessage}");
            }
        }

        //Read OrderDetails
        public async Task<IEnumerable<OrderDetails>> ReadOrderDetails(string? id, int page, int pageSize)
        {
            if (id == null)
            {
                IQueryable<OrderDetails> query = _unitOfWork.GetRepository<OrderDetails>().Entities
                    .Where(detail => detail.DeletedTime == null)
                    .OrderBy(detail => detail.OrderID);

                var paginated = await _unitOfWork.GetRepository<OrderDetails>()
                    .GetPagging(query, page, pageSize);

                return paginated.Items;
            }
            else
            {
                OrderDetails? od = await _unitOfWork.GetRepository<OrderDetails>()
                    .Entities
                    .FirstOrDefaultAsync(or => or.Id == id && or.DeletedTime == null);
                if (od == null)
                {
                    throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, $"Order detail with {od} not found!");
                }
                return new List<OrderDetails> { od };
            }
        }

        // Update OrderDetails
        public async Task<OrderDetails> UpdateOrderDetails(string id, OrderDetailsModelView model)
        {
            // Kiểm tra số lượng từ model
            if (model.Quantity <= 0 || model.Quantity % 1 != 0)
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, "Quantity must be greater than 0 and an integer.");
            }

            //OrderDetails orderDetails = await _unitOfWork.GetRepository<OrderDetails>().GetAllAsync()
            //    .ContinueWith(task => task.Result.FirstOrDefault(od => od.OrderID == orderId && od.ProductID == productId));
            OrderDetails? orderDetails = await _unitOfWork.GetRepository<OrderDetails>().GetByIdAsync(id);
            if (orderDetails == null)
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, $"Order detail with {orderDetails} not found!");
            }

            string productID = orderDetails.ProductID;
            Products? product = await _unitOfWork.GetRepository<Products>().GetByIdAsync(productID);
            if (product == null)
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, $"Product with {product} not found!");
            }

            orderDetails.Quantity = model.Quantity;
            orderDetails.UnitPrice = product.Price;

            await _unitOfWork.GetRepository<OrderDetails>().UpdateAsync(orderDetails);
            await _unitOfWork.SaveAsync();

            await _orderService.UpdateToTalAmount(orderDetails.OrderID);
            return orderDetails;
        }

        // Delete OrderDetails by OrderID and ProductID
        public async Task DeleteOrderDetails(string id)
        {
            //OrderDetails od = await _unitOfWork.GetRepository<OrderDetails>().GetAllAsync()
            //    .ContinueWith(task => task.Result.FirstOrDefault(od => od.OrderID == orderId && od.ProductID == productId));
            OrderDetails? od = await _unitOfWork.GetRepository<OrderDetails>().GetByIdAsync(id);
            if (od == null)
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, $"Order detail with {od} not found!");
            }

            od.DeletedTime = CoreHelper.SystemTimeNow;
            await _unitOfWork.GetRepository<OrderDetails>().UpdateAsync(od);
            await _unitOfWork.SaveAsync();
            await _orderService.UpdateToTalAmount(od.OrderID);
        }
    }
}
