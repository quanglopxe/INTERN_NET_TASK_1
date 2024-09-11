using Microsoft.EntityFrameworkCore;
using MilkStore.Contract.Repositories.IUOW;
using MilkStore.ModelViews.OrderModelViews;
using MilkStore.Contract.Repositories.Entity;
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

        public async Task<Order> GetByIdAsync(string id)
        {
            return await _context.Orders.FindAsync(id);
        }

        public async Task AddAsync(OrderModelView ord)
        {
            var item = new Order
            {
                UserId = ord.UserId,
                VoucherId = ord.VoucherId,
                TotalAmount = ord.TotalAmount,
                ShippingAddress = ord.ShippingAddress,
                Status = ord.Status,
                PaymentMethod = ord.PaymentMethod,
                OrderDate = ord.OrderDate,
            };
            await _context.Orders.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(string id, OrderModelView ord)
        {
            var orderss = await GetByIdAsync(id);
            orderss.UserId = ord.UserId;
            orderss.VoucherId = ord.VoucherId;
            orderss.TotalAmount = ord.TotalAmount;
            orderss.ShippingAddress = ord.ShippingAddress;
            orderss.Status = ord.Status;
            orderss.PaymentMethod = ord.PaymentMethod;
            orderss.OrderDate = ord.OrderDate;
            _context.Orders.Update(orderss);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var ord = await _context.Orders.FindAsync(id);
            if (ord != null)
            {
                _context.Orders.Remove(ord);
                await _context.SaveChangesAsync();
            }
        }
    }
}
