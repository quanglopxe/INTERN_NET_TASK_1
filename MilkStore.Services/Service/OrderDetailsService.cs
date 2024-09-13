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

namespace MilkStore.Services.Service
{
    public class OrderDetailsService : IOrderDetailsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DatabaseContext _context;
        protected readonly DbSet<OrderDetails> _dbSet;

        public OrderDetailsService(IUnitOfWork unitOfWork, DatabaseContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _dbSet = _context.Set<OrderDetails>();
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

            await _dbSet.AddAsync(orderDetails);
            await _unitOfWork.SaveAsync();
        }

        //Read OrderDetails
        public async Task<IEnumerable<OrderDetails>> ReadOrderDetails(string? orderId, int page = 1, int pageSize = 10)
        {
            IQueryable<OrderDetails> query = _dbSet.Where(e => EF.Property<DateTimeOffset?>(e, "DeletedTime") == null); // Lọc các bản ghi chưa bị xóa mềm

            if (!string.IsNullOrEmpty(orderId))
            {
                if (Guid.TryParse(orderId, out Guid parsedOrderId))
                {
                    query = query.Where(od => od.OrderID == parsedOrderId.ToString());
                }
                else
                {
                    throw new ArgumentException("Order ID is not a valid GUID.");
                }
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (page > totalPages && totalPages > 0)
            {
                page = totalPages;
            }

            if (totalPages == 0)
            {
                page = 1;
                totalPages = 1;
            }

            var orderDetailsPaged = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return orderDetailsPaged;
        }

        // Update OrderDetails
        public async Task UpdateOrderDetails(OrderDetailsModelView model)
        {
            var orderDetails = await _dbSet.FirstOrDefaultAsync(od => od.OrderID == model.OrderID && od.ProductID == model.ProductID);
            if (orderDetails != null)
            {
                orderDetails.Quantity = model.Quantity;
                //orderDetails.UnitPrice = model.UnitPrice; //muốn thay đổi giá sản phẩm thì đổi bên Product
                //orderDetails.TotalAmount = model.Quantity * model.UnitPrice; // tính tự động

                _dbSet.Update(orderDetails);
                await _unitOfWork.SaveAsync();
            }
        }

        // Delete OrderDetails by OrderID and ProductID
        public async Task DeleteOrderDetails(string orderId, string productId)
        {
            if (Guid.TryParse(orderId, out Guid parsedOrderId) && Guid.TryParse(productId, out Guid parsedProductId))
            {
                var orderDetails = await _dbSet.FirstOrDefaultAsync(od => od.OrderID == parsedOrderId.ToString() && od.ProductID == parsedProductId.ToString());
                if (orderDetails != null)
                {
                    orderDetails.DeletedTime = CoreHelper.SystemTimeNow; // Gán thời gian xóa
                    // orderDetails.DeletedBy = deletedBy; // Gán người thực hiện nếu cần
                    orderDetails.LastUpdatedTime = CoreHelper.SystemTimeNow; // Cập nhật thời gian thay đổi cuối cùng
                    _dbSet.Update(orderDetails); // Cập nhật lại bản ghi
                    await _unitOfWork.SaveAsync();
                }
            }
            else
            {
                throw new ArgumentException("Order ID or Product ID is not a valid GUID.");
            }
        }
    }
}
