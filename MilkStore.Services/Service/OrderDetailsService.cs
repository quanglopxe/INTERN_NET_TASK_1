// using Microsoft.EntityFrameworkCore;
// using MilkStore.Contract.Repositories.Entity;
// using MilkStore.Contract.Repositories.Interface;
// using MilkStore.ModelViews.OrderDetailsModelView;
// using MilkStore.Repositories.Context;
// using MilkStore.ModelViews;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using MilkStore.Core.Utils;
// using MilkStore.Contract.Services.Interface;
// using MilkStore.ModelViews.OrderModelViews;
// using AutoMapper;
// using MilkStore.ModelViews.ResponseDTO;
// using MilkStore.Core.Base;
// using MailKit.Search;

// namespace MilkStore.Services.Service
// {
//     public class OrderDetailsService : IOrderDetailsService
//     {
//         private readonly IUnitOfWork _unitOfWork;
//         private readonly IOrderService _orderService;
//         private readonly DatabaseContext _context;
//         private readonly IMapper _mapper;


//         public OrderDetailsService(IUnitOfWork unitOfWork, DatabaseContext context, IOrderService orderService, IMapper mapper)
//         {
//             _unitOfWork = unitOfWork;
//             _context = context;
//             _orderService = orderService;
//             _mapper = mapper;
//         }

//         private OrderDetailResponseDTO MapToOrderDetailResponseDTO(OrderDetails details)
//         {
//             return _mapper.Map<OrderDetailResponseDTO>(details);
//         }

//         // Create OrderDetails
//         public async Task<OrderDetails> CreateOrderDetails(OrderDetailsModelView model)
//         {
//             try
//             {
//                 // Kiểm tra xem số lượng có hợp lệ không
//                 if (model.Quantity <= 0 || model.Quantity % 1 != 0)
//                 {
//                     throw new BaseException.ErrorException(400, "BadRequest", "Số lượng phải lớn hơn 0 và là số nguyên.");
//                 }

//                 // Truy cập trực tiếp vào DbContext để tìm sản phẩm
//                 Products product = await _context.Products.FirstOrDefaultAsync(p => p.Id == model.ProductID);
//                 if (product == null)
//                 {
//                     throw new BaseException.ErrorException(404, "NotFound", $"Sản phẩm với ID: {product} không tồn tại.");
//                 }

//                 // Kiểm tra xem OrderDetails đã tồn tại hay chưa dựa trên OrderID và ProductID
//                 OrderDetails existingOrderDetail = await _context.OrderDetails
//                     .FirstOrDefaultAsync(od => od.OrderID == model.OrderID && od.ProductID == model.ProductID);

//                 if (existingOrderDetail != null)
//                 {
//                     // Nếu đã tồn tại, cập nhật số lượng và tính lại tổng tiền
//                     existingOrderDetail.Quantity += model.Quantity;
//                 }
//                 else
//                 {
//                     // Nếu chưa tồn tại, tạo OrderDetails mới
//                     OrderDetails orderDetails = _mapper.Map<OrderDetails>(model);
//                     orderDetails.UnitPrice = product.Price;

//                     // Thêm mới OrderDetails
//                     _context.OrderDetails.Add(orderDetails);
//                     existingOrderDetail = orderDetails;
//                 }
//                 await _context.SaveChangesAsync();

//                 // Cập nhật tổng giá trị đơn hàng
//                 await _orderService.UpdateToTalAmount(model.OrderID);
//                 return existingOrderDetail;
//             }
//             catch (Exception ex)
//             {
//                 string innerExceptionMessage = ex.InnerException?.Message ?? ex.Message;
//                 throw new BaseException.ErrorException(500, "InternalServerError", $"Đã xảy ra lỗi: {innerExceptionMessage}");
//             }
//         }

//         //Read OrderDetails
//         public async Task<IEnumerable<OrderDetails>> ReadOrderDetails(string? id, int page, int pageSize)
//         {
//             if (id == null)
//             {
//                 IQueryable<OrderDetails> query = _unitOfWork.GetRepository<OrderDetails>().Entities
//                     .Where(detail => detail.DeletedTime == null)
//                     .OrderBy(detail => detail.OrderID);

//                 var paginated = await _unitOfWork.GetRepository<OrderDetails>()
//                     .GetPagging(query, page, pageSize);

//                 return paginated.Items;
//             }
//             else
//             {
//                 OrderDetails? od = await _unitOfWork.GetRepository<OrderDetails>()
//                     .Entities
//                     .FirstOrDefaultAsync(or => or.OrderID == id && or.DeletedTime == null);
//                 if (od == null)
//                 {
//                     throw new BaseException.ErrorException(404, "NotFound", $"Không tìm thấy đơn theo ID: {od} .");
//                 }
//                 return new List<OrderDetails> { od };
//             }
//         }

//         // Update OrderDetails
//         public async Task<OrderDetails> UpdateOrderDetails(string orderId, string productId, OrderDetailsModelView model)
//         {
//             // Kiểm tra số lượng từ model
//             if (model.Quantity <= 0 || model.Quantity % 1 != 0)
//             {
//                 throw new BaseException.ErrorException(400, "BadRequest", "Số lượng phải lớn hơn 0 và là số nguyên.");
//             }

//             OrderDetails orderDetails = await _unitOfWork.GetRepository<OrderDetails>().GetAllAsync()
//                 .ContinueWith(task => task.Result.FirstOrDefault(od => od.OrderID == orderId && od.ProductID == productId));
//             if (orderDetails == null)
//             {
//                 throw new BaseException.ErrorException(404, "NotFound", $"Order Details không tồn tại cho OrderID: {orderId} và ProductID: {productId}.");
//             }

//             // Cập nhật thông tin
//             Products product = await _unitOfWork.GetRepository<Products>().GetByIdAsync(productId);
//             if (product == null)
//             {
//                 throw new BaseException.ErrorException(404, "NotFound", $"Sản phẩm không tồn tại với ID: {productId}.");
//             }

//             orderDetails.Quantity = model.Quantity;
//             orderDetails.UnitPrice = product.Price;

//             await _unitOfWork.GetRepository<OrderDetails>().UpdateAsync(orderDetails);
//             await _unitOfWork.SaveAsync();

//             await _orderService.UpdateToTalAmount(orderDetails.OrderID);
//             return orderDetails;
//         }

//         // Delete OrderDetails by OrderID and ProductID
//         public async Task DeleteOrderDetails(string orderId, string productId)
//         {
//             OrderDetails od = await _unitOfWork.GetRepository<OrderDetails>().GetAllAsync()
//                 .ContinueWith(task => task.Result.FirstOrDefault(od => od.OrderID == orderId && od.ProductID == productId));
//             if (od == null)
//             {
//                 throw new BaseException.ErrorException(Core.Constants.StatusCodes.NotFound, "NotFound", $"Order Details không tồn tại cho OrderID: {orderId} và ProductID: {productId}.");
//             }

//             od.DeletedTime = CoreHelper.SystemTimeNow;
//             await _unitOfWork.GetRepository<OrderDetails>().UpdateAsync(od);
//             await _unitOfWork.SaveAsync();
//             await _orderService.UpdateToTalAmount(od.OrderID);
//         }
//     }
// }
