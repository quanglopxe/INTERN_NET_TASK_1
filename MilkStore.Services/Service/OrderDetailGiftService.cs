using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Repositories.Interface;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core.Base;
using MilkStore.Core.Utils;
using MilkStore.ModelViews.GiftModelViews;
using MilkStore.ModelViews.OrderDetailGiftModelView;
using MilkStore.Repositories.Context;
using MilkStore.Repositories.Entity;
using MilkStore.Services.EmailSettings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.Services.Service
{
    public class OrderDetailGiftService : IOrderDetailGiftService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DatabaseContext context;
        private readonly IMapper _mapper;
        public OrderDetailGiftService(DatabaseContext context, IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.context = context;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<OrderDetailGift> CreateOrderDetailGift(OrderDetailGiftModel orderDetailGiftModel)
        {
            OrderDetailGift newOrderDetailGift = _mapper.Map<OrderDetailGift>(orderDetailGiftModel);
            IEnumerable<OrderDetailGift> newODG = await _unitOfWork.GetRepository<OrderDetailGift>().GetAllAsync();
            //Gift gift = await _unitOfWork.GetRepository<Gift>().GetByIdAsync(newOrderDetailGift.GiftId);
            //OrderGift og = await _unitOfWork.GetRepository<OrderGift>().GetByIdAsync(newOrderDetailGift.OrderGiftId);
            //int temp1 = gift.point;// so sánh points để thỏa điều kiện
            //int temp2 = og.User.Points;// so sánh points để thỏa điều kiện
            foreach (var item in newODG)
            {
                if (item.OrderGiftId == orderDetailGiftModel.OrderGiftId)
                {
                    Console.WriteLine("OrdergiftID: " + item.OrderGiftId);
                    if (item.GiftId == orderDetailGiftModel.GiftId)
                    {
                        Console.WriteLine("GiftId: " + orderDetailGiftModel.GiftId);
                        OrderDetailGift newOrderDetailGift1 = await _unitOfWork.GetRepository<OrderDetailGift>().GetByIdAsync(item.Id);
                        newOrderDetailGift1.quantity += orderDetailGiftModel.quantity;
                        newOrderDetailGift1.LastUpdatedTime = DateTime.UtcNow;
                        await _unitOfWork.GetRepository<OrderDetailGift>().UpdateAsync(newOrderDetailGift1);
                        await _unitOfWork.SaveAsync();
                        return newOrderDetailGift1;
                    }

                }
            }
            if (orderDetailGiftModel.id == null || orderDetailGiftModel.id == "")
            {
                newOrderDetailGift.Id = Guid.NewGuid().ToString("N");
            }
            // Đặt các thuộc tính mặc định (nếu có)
            newOrderDetailGift.CreatedTime = DateTime.UtcNow;
            newOrderDetailGift.LastUpdatedTime = DateTime.UtcNow;
            newOrderDetailGift.DeletedTime = null; // Khi tạo mới thì không có DeletedTime

            // Thêm mới vào cơ sở dữ liệu
            await _unitOfWork.GetRepository<OrderDetailGift>().InsertAsync(newOrderDetailGift);
            await _unitOfWork.SaveAsync();
            return newOrderDetailGift;
        }

        //public async Task<OrderDetailGift> Check_PointGift(OrderDetailGiftModel orderDetailGiftModel)
        //{
        //    OrderDetailGift ODG = _mapper.Map<OrderDetailGift>(orderDetailGiftModel);
        //    OrderGift og = await _unitOfWork.GetRepository<OrderGift>().GetByIdAsync(orderDetailGiftModel.OrderGiftId);
        //    Gift g = await _unitOfWork.GetRepository<Gift>().GetByIdAsync(orderDetailGiftModel.GiftId);

        //    ApplicationUser user = await _unitOfWork.GetRepository<ApplicationUser>().GetByIdAsync(og.UserID);

        //    if (user.Points >= g.point)
        //    {
        //        return og;
        //    }
        //    return og;
        //}
        public async Task<OrderDetailGift> DeleteOrderDetailGift(object id)
        {
            try
            {
                // Lấy OrderDetailGift hiện tại từ cơ sở dữ liệu
                OrderDetailGift existingOrderDetailGift = await _unitOfWork.GetRepository<OrderDetailGift>().GetByIdAsync(id);
                if (existingOrderDetailGift == null)
                    //throw new BaseException.ErrorException(400, "Bad Request", "Không tồn tại id!!!");

                // Đánh dấu là đã xóa
                existingOrderDetailGift.DeletedTime = DateTime.UtcNow;

                await _unitOfWork.GetRepository<OrderDetailGift>().UpdateAsync(existingOrderDetailGift);
                await _unitOfWork.SaveAsync();

                return existingOrderDetailGift;
            }
            catch (BaseException.ErrorException e)
            {
                throw new Exception($"Error");
            }
        }

        public async Task<IEnumerable<OrderDetailGiftModel>> GetOrderDetailGift(string? id)
        {
            if (id == null)
            {
                // Lấy tất cả sản phẩm
                IEnumerable<OrderDetailGift> Gift = await _unitOfWork.GetRepository<OrderDetailGift>().GetAllAsync();

                // Lọc sản phẩm có DeleteTime == null
                Gift = Gift.Where(p => p.DeletedTime == null);

                return _mapper.Map<IEnumerable<OrderDetailGiftModel>>(Gift);
            }
            else
            {
                // Lấy sản phẩm theo ID
                Gift Gift = await _unitOfWork.GetRepository<Gift>().GetByIdAsync(id);

                if (Gift != null && Gift.DeletedTime == null) // Kiểm tra DeleteTime
                {
                    return new List<OrderDetailGiftModel> { _mapper.Map<OrderDetailGiftModel>(Gift) };
                }
                else
                {
                    return new List<OrderDetailGiftModel>();
                }
            }
        }


        public async Task<OrderDetailGift> UpdateOrderDetailGift(string id, OrderDetailGiftModel OrderDetailGiftModel)
        {
            OrderDetailGift existingGift = await _unitOfWork.GetRepository<OrderDetailGift>().GetByIdAsync(id);

            if (existingGift == null)
            {
                throw new Exception("Sản phẩm không tồn tại.");
            }

            // Cập nhật thông tin sản phẩm bằng cách ánh xạ từ DTO
            _mapper.Map(OrderDetailGiftModel, existingGift);
            existingGift.LastUpdatedTime = DateTime.UtcNow;

            await _unitOfWork.GetRepository<OrderDetailGift>().UpdateAsync(existingGift);
            await _unitOfWork.SaveAsync();

            return existingGift;
        }
    }
}
