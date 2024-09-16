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
using System.Text.Json.Serialization;
using System.Text.Json;
using MilkStore.ModelViews.ResponseDTO;
using System.ComponentModel;

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

        private OrderResponseDTO MapToOrderResponseDto(Order order)
        {
            return new OrderResponseDTO
            {
                Id = order.Id,
                UserId = order.UserId,
                VoucherId = order.VoucherId,
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                DiscountedAmount = order.DiscountedAmount,
                ShippingAddress = order.ShippingAddress,
                PaymentMethod = order.PaymentMethod,
                OrderDetailss = order.OrderDetailss.Select(pp => new OrderDetailResponseDTO
                {
                    ProductID = pp.ProductID,
                    Quantity = pp.Quantity,
                    UnitPrice = pp.UnitPrice
                }).ToList()
            };
        }

        


        public async Task<IEnumerable<OrderResponseDTO>> GetAsync(string? id)
        {
            if(id == null)
            {
                //return await _unitOfWork.GetRepository<Order>().GetAllAsync();
                var listOrder = await _dbSet.Where(e => EF.Property<DateTimeOffset?>(e, "DeletedTime") == null).ToListAsync();
                return listOrder.Select(MapToOrderResponseDto).ToList();
            }
            else
            {
                var ord = await _unitOfWork.GetRepository<Order>().Entities.FirstOrDefaultAsync(or => or.Id == id && or.DeletedTime == null);
                if (ord == null)
                {
                    throw new KeyNotFoundException($"Post with ID {id} was not found.");
                }
                return new List<OrderResponseDTO> { MapToOrderResponseDto(ord) };
            }
        }

        public async Task<Order> AddAsync(OrderModelView ord)
        {
            var item = new Order
            {
                UserId = ord.UserId,
                VoucherId = ord.VoucherId,
                TotalAmount = 0,
                DiscountedAmount = 0,
                ShippingAddress = ord.ShippingAddress,
                Status = ord.Status,
                PaymentMethod = ord.PaymentMethod,
                OrderDate = ord.OrderDate,
            };
            await _unitOfWork.GetRepository<Order>().InsertAsync(item);
            await _unitOfWork.SaveAsync();
            return item;
        }

        public async Task<Order> UpdateAsync(string id, OrderModelView ord)
        {
            var orderss = await _unitOfWork.GetRepository<Order>().GetByIdAsync(id);
            orderss.UserId = ord.UserId;
            orderss.VoucherId = ord.VoucherId;
            orderss.ShippingAddress = ord.ShippingAddress;
            orderss.Status = ord.Status;
            orderss.PaymentMethod = ord.PaymentMethod;
            orderss.OrderDate = ord.OrderDate;
            orderss.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<Order>().UpdateAsync(orderss);
            await _unitOfWork.SaveAsync();
            return orderss;
        }

        //Cập nhật TotalAmount
        public async Task UpdateToTalAmount (string id, double amount)
        {
            var ord = await _unitOfWork.GetRepository<Order>().GetByIdAsync(id);
            ord.TotalAmount += amount;
            ord.DiscountedAmount = ord.TotalAmount ;
            //Tính thành tiền áp dụng ưu đãi
            var vch = await _unitOfWork.GetRepository<Voucher>().Entities.FirstOrDefaultAsync(vc => vc.Id == ord.VoucherId && vc.DeletedTime == null);
            if (vch != null)
            {
                if (vch.ExpiryDate > ord.OrderDate && Convert.ToDouble(vch.LimitSalePrice) <= amount && vch.UsedCount < vch.UsingLimit)
                {
                    var discountAmount = (ord.TotalAmount * vch.SalePercent) / 100.0;
                    vch.UsedCount++;

                    await _unitOfWork.GetRepository<Voucher>().UpdateAsync(vch);
                    ord.DiscountedAmount = ord.TotalAmount - discountAmount;
                }
            }
            await _unitOfWork.GetRepository<Order>().UpdateAsync(ord);
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
