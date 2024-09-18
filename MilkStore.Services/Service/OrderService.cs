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

namespace MilkStore.Services.Service
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DatabaseContext _context;
        protected readonly DbSet<Order> _dbSet;
        private readonly IUserService _userService;

        public OrderService(IUnitOfWork unitOfWork, DatabaseContext context, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _dbSet = _context.Set<Order>();
            _userService = userService;
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
                var ord = await _unitOfWork.GetRepository<Order>().Entities.FirstOrDefaultAsync(or => or.Id == id && or.DeletedTime == null);
                return ord != null ? new List<Order> { ord } : new List<Order>();
            }
        }

        public async Task AddAsync(OrderModelView ord)
        {
            var item = new Order
            {
                UserId = ord.UserId,
                VoucherId = ord.VoucherId,
                TotalAmount = 0,
                ShippingAddress = ord.ShippingAddress,
                Status = ord.Status,
                PaymentMethod = ord.PaymentMethod,
                OrderDate = ord.OrderDate,
            };

            // Nếu đơn hàng có trạng thái "Completed", tích lũy điểm cho người dùng
            if (item.Status == "Completed" && item.TotalAmount >= 10000)
            {
                item.PointsAdded = 1;
                await _userService.AccumulatePoints(item.UserId, item.TotalAmount);
            }

            await _unitOfWork.GetRepository<Order>().InsertAsync(item);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(string id, OrderModelView ord)
        {
            var orderss = await _unitOfWork.GetRepository<Order>().GetByIdAsync(id);
            orderss.UserId = ord.UserId;
            orderss.VoucherId = ord.VoucherId;
            orderss.ShippingAddress = ord.ShippingAddress;
            orderss.Status = ord.Status;
            orderss.PaymentMethod = ord.PaymentMethod;
            orderss.OrderDate = ord.OrderDate;
            orderss.LastUpdatedTime = CoreHelper.SystemTimeNow;

            // Nếu đơn hàng có trạng thái "Completed", tích lũy điểm cho người dùng
            if (orderss.Status == "Completed" && orderss.TotalAmount >= 10000 && orderss.PointsAdded!=1)
            {
                orderss.PointsAdded = 1;
                await _userService.AccumulatePoints(orderss.UserId, orderss.TotalAmount);
            }

            await _unitOfWork.GetRepository<Order>().UpdateAsync(orderss);
            await _unitOfWork.SaveAsync();
        }

        //Cập nhật TotalAmount
        public async Task UpdateToTalAmount (string id, double amount)
        {
            var ord = await _unitOfWork.GetRepository<Order>().GetByIdAsync(id);
            //Tính thành tiền áp dụng ưu đãi
            var vch = await _unitOfWork.GetRepository<Voucher>().Entities.FirstOrDefaultAsync(vc => vc.Id == ord.VoucherId && vc.DeletedTime == null);
            if (vch != null)
            {
                if (vch.ExpiryDate > ord.OrderDate && vch.LimitSalePrice <= amount && vch.UsedCount < vch.UsingLimit)
                {
                    amount = amount - ((amount * vch.SalePercent) / (100 * 1.0));
                    vch.UsedCount = vch.UsedCount + 1;
                    await _unitOfWork.GetRepository<Voucher>().UpdateAsync(vch);
                }
            }
   
            ord.TotalAmount += amount;
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
