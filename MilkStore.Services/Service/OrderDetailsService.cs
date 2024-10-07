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
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPreOrdersService _preOrdersService;

        public OrderDetailsService(IUnitOfWork unitOfWork, IOrderService orderService, IMapper mapper, IHttpContextAccessor httpContextAccessor, IPreOrdersService preOrdersService)
        {
            _unitOfWork = unitOfWork;
            _orderService = orderService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _preOrdersService = preOrdersService;
        }
        //private OrderDetailResponseDTO MapToOrderDetailResponseDTO(OrderDetails details)
        //{
        //    return _mapper.Map<OrderDetailResponseDTO>(details);
        //}

        // Create OrderDetails check PreOrder
        public async Task<OrderDetailResponseDTO> CreateOrderDetails(OrderDetailsModelView model)
        {
            string? userID = _httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userID))
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.Unauthorized, ErrorCode.Unauthorized, "Please log in first!");
            }

            if (model.Quantity <= 0 || model.Quantity % 1 != 0)
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, "Quantity must be greater than 0 and an integer.");
            }

            Products product = await _unitOfWork.GetRepository<Products>().Entities
                .FirstOrDefaultAsync(p => p.Id == model.ProductID)
                ?? throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, "Product not found!");

            int availableQuantity = product.QuantityInStock;
            int requestedQuantity = model.Quantity;

            OrderDetailResponseDTO response = new OrderDetailResponseDTO
            {
                ProductID = product.Id,
                ProductName = product.ProductName,
                RequestedQuantity = requestedQuantity,
                AvailableQuantity = availableQuantity,
                UnitPrice = product.Price
            };

            if (availableQuantity < requestedQuantity)
            {
                response.PurchasedQuantity = availableQuantity;
                response.PreOrderQuantity = requestedQuantity - availableQuantity;
                response.Message = $"Only {availableQuantity} units are available. {response.PreOrderQuantity} units have been preordered. Please confirm to proceed.";
                response.IsConfirmationRequired = true;
                return response;
            }
            else
            {
                await CreateOrUpdateOrderDetails(model.OrderID, model.ProductID, requestedQuantity, product.Price, userID);
                response.PurchasedQuantity = requestedQuantity;
                response.Message = "Order processed successfully.";
                response.IsConfirmationRequired = false;
            }

            await _orderService.UpdateToTalAmount(model.OrderID);
            return response;
        }
        public async Task<OrderDetailResponseDTO> ConfirmOrderDetails(OrderDetailsConfirmationModel model)
        {
            string? userID = _httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userID))
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.Unauthorized, ErrorCode.Unauthorized, "Please log in first!");
            }

            Products product = await _unitOfWork.GetRepository<Products>().Entities
                .FirstOrDefaultAsync(p => p.Id == model.ProductID)
                ?? throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, "Product not found!");

            int availableQuantity = product.QuantityInStock;
            int requestedQuantity = model.Quantity;

            if (requestedQuantity > availableQuantity)
            {
                await CreateOrUpdateOrderDetails(model.OrderID, model.ProductID, availableQuantity, product.Price, userID);
                await CreatePreOrders(model.ProductID, requestedQuantity - availableQuantity, userID);

                return new OrderDetailResponseDTO
                {
                    ProductID = product.Id,
                    ProductName = product.ProductName,
                    RequestedQuantity = requestedQuantity,
                    PurchasedQuantity = availableQuantity,
                    PreOrderQuantity = requestedQuantity - availableQuantity,
                    UnitPrice = product.Price,
                    Message = $"Only {availableQuantity} units are available. {requestedQuantity - availableQuantity} units have been preordered."
                };
            }
            else
            {
                await CreateOrUpdateOrderDetails(model.OrderID, model.ProductID, requestedQuantity, product.Price, userID);

                return new OrderDetailResponseDTO
                {
                    ProductID = product.Id,
                    ProductName = product.ProductName,
                    RequestedQuantity = requestedQuantity,
                    PurchasedQuantity = requestedQuantity,
                    UnitPrice = product.Price,
                    Message = "Order confirmed and processed successfully."
                };
            }
        }

        private async Task CreatePreOrders(string productId, int quantity, string userID)
        {
            PreOrdersModelView preOrdersModelView = new PreOrdersModelView
            {
                ProductID = productId,
                Quantity = quantity,
                Status = PreOrderStatus.Pending,
                UserID = Guid.Parse(userID)
            };

            await _preOrdersService.CreatePreOrders(preOrdersModelView);
        }


        private async Task CreateOrUpdateOrderDetails(string orderId, string productId, int quantity, double price, string userID)
        {
            OrderDetails? existingOrderDetail = await _unitOfWork.GetRepository<OrderDetails>().Entities
                .FirstOrDefaultAsync(od => od.OrderID == orderId && od.ProductID == productId && od.DeletedTime == null);

            if (existingOrderDetail != null)
            {
                existingOrderDetail.Quantity += quantity;
                existingOrderDetail.UnitPrice = price;
                existingOrderDetail.LastUpdatedBy = userID;
                existingOrderDetail.LastUpdatedTime = CoreHelper.SystemTimeNow;
            }
            else
            {
                OrderDetails newOrderDetails = new OrderDetails
                {
                    OrderID = orderId,
                    ProductID = productId,
                    Quantity = quantity,
                    UnitPrice = price,
                    CreatedBy = userID,
                    CreatedTime = CoreHelper.SystemTimeNow
                };

                await _unitOfWork.GetRepository<OrderDetails>().InsertAsync(newOrderDetails);
            }

            await _unitOfWork.SaveAsync();
        }



        //Read OrderDetails
        public async Task<IEnumerable<OrderDetails>> ReadOrderDetails(string? id, int page, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(id))
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
                OrderDetails? od = await _unitOfWork.GetRepository<OrderDetails>().Entities
                    .FirstOrDefaultAsync(or => or.Id == id && or.DeletedTime == null) 
                    ?? throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, $"Order detail with {id} not found!");
                
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
            OrderDetails? orderDetails = await _unitOfWork.GetRepository<OrderDetails>().GetByIdAsync(id)
                ?? throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, $"Order detail with {id} not found!");

            string productID = orderDetails.ProductID;
            Products? product = await _unitOfWork.GetRepository<Products>().GetByIdAsync(productID)
                ?? throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, $"Product with {productID} not found!");
            
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
            OrderDetails? od = await _unitOfWork.GetRepository<OrderDetails>().GetByIdAsync(id)
                ?? throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, $"Order detail with {id} not found!");

            od.DeletedTime = CoreHelper.SystemTimeNow;
            await _unitOfWork.GetRepository<OrderDetails>().UpdateAsync(od);
            await _unitOfWork.SaveAsync();
            await _orderService.UpdateToTalAmount(od.OrderID);
        }
    }
}
