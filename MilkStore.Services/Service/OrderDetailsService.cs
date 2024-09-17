using Microsoft.EntityFrameworkCore;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Repositories.Interface;
using MilkStore.ModelViews.OrderDetailsModelView;
using MilkStore.Repositories.Context;
using MilkStore.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MilkStore.Core.Utils;
using MilkStore.Contract.Services.Interface;
using MilkStore.ModelViews.OrderModelViews;

namespace MilkStore.Services.Service
{
    public class OrderDetailsService : IOrderDetailsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderService _orderService;
        private readonly DatabaseContext _context;
        protected readonly DbSet<OrderDetails> _dbSet;

        public OrderDetailsService(IUnitOfWork unitOfWork, DatabaseContext context, IOrderService orderService)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _dbSet = _context.Set<OrderDetails>();
            _orderService = orderService;
        }

        // Create OrderDetails
        public async Task CreateOrderDetails(OrderDetailsModelView model)
        {
            var orderDetails = new OrderDetails
            {
                OrderID = model.OrderID,
                ProductID = model.ProductID,
                Quantity = model.Quantity,
                UnitPrice = model.UnitPrice,
                //TotalAmount = model.Quantity * model.UnitPrice
            };
            //Update lên bảng Order
            
            await _dbSet.AddAsync(orderDetails);
            await _unitOfWork.SaveAsync();
            await _orderService.UpdateToTalAmount(orderDetails.OrderID);
        }

        //Read OrderDetails
        public async Task<IEnumerable<OrderDetails>> ReadOrderDetails(string? orderId, int page = 1, int pageSize = 10)
        {
            IQueryable<OrderDetails> query = _dbSet.Where(e => EF.Property<DateTimeOffset?>(e, "DeletedTime") == null); // Lọc các bản ghi chưa bị xóa mềm

            if (!string.IsNullOrEmpty(orderId))
            {
                query = query.Where(od => od.OrderID == orderId);
            }

            // Tính toán phân trang
            var totalItems = await query.CountAsync();
            var totalPages = totalItems > 0 ? (int)Math.Ceiling((double)totalItems / pageSize) : 1;
            page = Math.Max(1, Math.Min(page, totalPages));

            var orderDetailsPaged = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return orderDetailsPaged;
        }

        // Update OrderDetails
        public async Task UpdateOrderDetails(string id, OrderDetailsModelView model)
        {
            var orderDetails = await _unitOfWork.GetRepository<OrderDetails>().GetByIdAsync(id);
            if (orderDetails != null)
            {
                //orderDetails.OrderID = model.OrderID;
                orderDetails.ProductID = model.ProductID;
                orderDetails.Quantity = model.Quantity;
                orderDetails.UnitPrice = model.UnitPrice;
                //orderDetails.TotalAmount = model.Quantity * model.UnitPrice; // tính tự động
                //await _orderService.UpdateToTalAmount(orderDetails.OrderID, orderDetails.TotalAmount);
                _dbSet.Update(orderDetails);
                await _unitOfWork.SaveAsync();
                await _orderService.UpdateToTalAmount(orderDetails.OrderID);
            }
        }

        // Delete OrderDetails by OrderID and ProductID
        public async Task DeleteOrderDetails(string id)
        {
            var od = await _unitOfWork.GetRepository<OrderDetails>().GetByIdAsync(id);
            od.DeletedTime = CoreHelper.SystemTimeNow;
            await _unitOfWork.GetRepository<OrderDetails>().UpdateAsync(od);
            await _unitOfWork.SaveAsync();
            await _orderService.UpdateToTalAmount(od.OrderID);
        }
    }
}
