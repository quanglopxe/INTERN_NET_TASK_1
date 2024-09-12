using MilkStore.Contract.Repositories;
using MilkStore.Contract.Services.Interface;
using MilkStore.ModelViews.OrderModelViews;
using MilkStore.Contract.Repositories.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using MilkStore.Contract.Repositories.Interface;
using MilkStore.Core.Utils;
using MilkStore.Repositories.Context;
using Microsoft.EntityFrameworkCore;

namespace MilkStore.Services.Service
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DatabaseContext _context;
        protected readonly DbSet<Order> _dbSet;

        public OrderService(IUnitOfWork unitOfWork, DatabaseContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _dbSet = _context.Set<Order>();
        }

        public async Task<IEnumerable<Order>> GetAsync(string? id)
        {
            if(id == null)
            {
                //return await _unitOfWork.GetRepository<Order>().GetAllAsync();
                return await _dbSet.Where(e => EF.Property<DateTimeOffset?>(e, "DeletedTime") == null).ToListAsync();
            }
            else
            {
                var ord =  await _unitOfWork.GetRepository<Order>().GetByIdAsync(id);
                return ord != null ? new List<Order> { ord } : new List<Order>();
            }
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
            await _unitOfWork.GetRepository<Order>().InsertAsync(item);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(string id, OrderModelView ord)
        {
            var orderss = await _unitOfWork.GetRepository<Order>().GetByIdAsync(id);
            orderss.UserId = ord.UserId;
            orderss.VoucherId = ord.VoucherId;
            orderss.TotalAmount = ord.TotalAmount;
            orderss.ShippingAddress = ord.ShippingAddress;
            orderss.Status = ord.Status;
            orderss.PaymentMethod = ord.PaymentMethod;
            orderss.OrderDate = ord.OrderDate;
            orderss.LastUpdatedTime = CoreHelper.SystemTimeNow;
            await _unitOfWork.GetRepository<Order>().UpdateAsync(orderss);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var orderss = await _unitOfWork.GetRepository<Order>().GetByIdAsync(id);
            orderss.DeletedTime = CoreHelper.SystemTimeNow;
            await _unitOfWork.GetRepository<Order>().UpdateAsync(orderss);
            await _unitOfWork.SaveAsync();
        }
    }
}
