using Microsoft.EntityFrameworkCore;
using MilkStore.Contract.Repositories.IUOW;
using MilkStore.ModelViews.OrderModelViews;
using MilkStore.Core.Model;
using MilkStore.Repositories.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.Repositories.UOW
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DatabaseContext _context;
        public OrderRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders.ToListAsync();
        }

        public async Task<Order> GetByIdAsync(Guid id)
        {
            return await _context.Orders.FindAsync(id);
        }

        public async Task AddAsync(OrderModelView product)
        {
            var item = new Order
            {
                UserId = product.UserId,
                VoucherId = product.VoucherId,
                TotalAmount = product.TotalAmount,
                ShippingAddress = product.ShippingAddress,
                Status = product.Status,
                PaymentMethod = product.PaymentMethod,
                OrderDate = product.OrderDate,
            };
            await _context.Orders.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Guid id, OrderModelView product)
        {
            var orderss = await GetByIdAsync(id);
            orderss.UserId = product.UserId;
            orderss.VoucherId = product.VoucherId;
            orderss.TotalAmount = product.TotalAmount;
            orderss.ShippingAddress = product.ShippingAddress;
            orderss.Status = product.Status;
            orderss.PaymentMethod = product.PaymentMethod;
            orderss.OrderDate = product.OrderDate;
            _context.Orders.Update(orderss);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var product = await _context.Orders.FindAsync(id);
            if (product != null)
            {
                _context.Orders.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}
