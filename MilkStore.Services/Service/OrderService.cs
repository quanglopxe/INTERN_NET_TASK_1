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

namespace MilkStore.Services.Service
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Order>> GetAsync(string? id)
        {
            if(id == null)
            {
                return await _unitOfWork.GetRepository<Order>().GetAllAsync();
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
            await _unitOfWork.GetRepository<Order>().UpdateAsync(orderss);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(string id)
        {
            await _unitOfWork.GetRepository<Order>().DeleteAsync(id);
            await _unitOfWork.SaveAsync();
        }
    }
}
