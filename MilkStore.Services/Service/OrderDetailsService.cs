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
        //protected readonly DbSet<OrderDetails> _dbSet;

        public OrderDetailsService(IUnitOfWork unitOfWork, DatabaseContext context, IOrderService orderService)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            //_dbSet = _context.Set<OrderDetails>();
            _orderService = orderService;
        }

        // Create OrderDetails
        public async Task<OrderDetails> CreateOrderDetails(OrderDetailsModelView model)
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
            await _orderService.UpdateToTalAmount(orderDetails.OrderID, orderDetails.TotalAmount);

            await _unitOfWork.GetRepository<OrderDetails>().InsertAsync(orderDetails);
            await _unitOfWork.SaveAsync();
            return orderDetails;
        }

        //Read OrderDetails
        public async Task<IEnumerable<OrderDetails>> ReadOrderDetails(string? id, int page, int pageSize)
        {
            if (id == null)
            {
                var query = _unitOfWork.GetRepository<OrderDetails>()
                    .Entities
                    .Where(detail => detail.DeletedTime == null)
                    .OrderBy(detail => detail.OrderID);
                    
                    
                var paginated = await _unitOfWork.GetRepository<OrderDetails>()
                    .GetPagging(query, page, pageSize);
                
                return paginated.Items;

            }
            else
            {
                var od = await _unitOfWork.GetRepository<OrderDetails>()
                    .Entities
                    .FirstOrDefaultAsync(or => or.Id == id && or.DeletedTime == null);
                if (od == null)
                {
                    throw new KeyNotFoundException($"Order Details have ID: {id} was not found.");
                }
                return new List<OrderDetails> { od };
            }
        }

        // Update OrderDetails
        public async Task<OrderDetails> UpdateOrderDetails(string id, OrderDetailsModelView model)
        {
            var orderDetails = await _unitOfWork.GetRepository<OrderDetails>().GetByIdAsync(id);
            if (orderDetails == null)
            {
                throw new KeyNotFoundException($"Order Details have ID: {id} was not found.");
            }                       
            //orderDetails.OrderID = model.OrderID;
            orderDetails.ProductID = model.ProductID;
            orderDetails.Quantity = model.Quantity;
            orderDetails.UnitPrice = model.UnitPrice;
            //orderDetails.TotalAmount = model.Quantity * model.UnitPrice;
            //await _orderService.UpdateToTalAmount(orderDetails.OrderID, orderDetails.TotalAmount);
            await _unitOfWork.GetRepository<OrderDetails>().UpdateAsync(orderDetails);
            await _unitOfWork.SaveAsync();
            return orderDetails;            
        }

        // Delete OrderDetails by OrderID and ProductID
        public async Task DeleteOrderDetails(string id)
        {
            var od = await _unitOfWork.GetRepository<OrderDetails>().GetByIdAsync(id);
            od.DeletedTime = CoreHelper.SystemTimeNow;
            await _unitOfWork.GetRepository<OrderDetails>().UpdateAsync(od);
            await _unitOfWork.SaveAsync();
        }
    }
}
