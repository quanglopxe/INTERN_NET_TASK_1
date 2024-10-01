using AutoMapper;
using MailKit;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Repositories.Interface;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core.Base;
using MilkStore.Core.Constants;
using MilkStore.ModelViews.GiftModelViews;
using MilkStore.ModelViews.OrderGiftModelViews;
using MilkStore.Repositories.Context;
using MilkStore.Repositories.Entity;
using MilkStore.Services.EmailSettings;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.Services.Service
{
    public class OrderGiftService : IOrderGiftService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DatabaseContext context;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        public OrderGiftService(DatabaseContext context, IUnitOfWork unitOfWork, IMapper mapper, IEmailService mailService)
        {
            this.context = context;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = mailService;
        }

        public async Task CreateOrderGift(OrderGiftModel orderGiftModel)
        {
            if (orderGiftModel.Id.Contains(" "))
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, "Error!!! Input wrong id");
            }
            OrderGift newOG = _mapper.Map<OrderGift>(orderGiftModel);
            if (newOG.Id == null || newOG.Id == "")
            {
                newOG.Id = Guid.NewGuid().ToString("N");
            }
            newOG.CreatedTime = DateTime.UtcNow;

            await _unitOfWork.GetRepository<OrderGift>().InsertAsync(newOG);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteOrderGift(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, "Error!!! Input wrong id");
            }
            OrderGift OrderGift1 = await _unitOfWork.GetRepository<OrderGift>().GetByIdAsync(id);

            if (OrderGift1.DeletedTime != null)
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, $"Doesn't exist:{id}");
            }
            OrderGift1.DeletedTime = DateTime.UtcNow;
            await _unitOfWork.GetRepository<OrderGift>().UpdateAsync(OrderGift1);
            await _unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<OrderGiftModel>> GetOrderGift(string? id)
        {
            if (id == null)
            {
                // Lấy tất cả sản phẩm
                IEnumerable<OrderGift> OGift = await _unitOfWork.GetRepository<OrderGift>().GetAllAsync();

                // Lọc sản phẩm có DeleteTime == null
                OGift = OGift.Where(p => p.DeletedTime == null);

                return _mapper.Map<IEnumerable<OrderGiftModel>>(OGift);
            }
            else
            {
                // Lấy sản phẩm theo ID
                OrderGift OGift = await _unitOfWork.GetRepository<OrderGift>().GetByIdAsync(id);

                if (OGift != null && OGift.DeletedTime == null) // Kiểm tra DeleteTime
                {
                    return new List<OrderGiftModel> { _mapper.Map<OrderGiftModel>(OGift) };
                }
                else
                {
                    return new List<OrderGiftModel>();
                }
            }
        }


        public async Task UpdateOrderGift(string id, OrderGiftModel orderGiftModel)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, "Error!!! Input wrong id");
            }

            OrderGift existingOGift = await _unitOfWork.GetRepository<OrderGift>().GetByIdAsync(id);

            if (existingOGift == null)
            {
                throw new BaseException.ErrorException(Core.Constants.StatusCodes.BadRequest, ErrorCode.BadRequest, $"Doesn't exist:{id}");
            }

            // Cập nhật thông tin sản phẩm bằng cách ánh xạ từ DTO
            _mapper.Map(orderGiftModel, existingOGift);
            existingOGift.LastUpdatedTime = DateTime.UtcNow;
            await _unitOfWork.GetRepository<OrderGift>().UpdateAsync(existingOGift);
            await _unitOfWork.SaveAsync();
        }
        public async Task SendMail_OrderGift(string? id)
        {
            DateTime currentDate = DateTime.Now;
            DateTime futureDate = currentDate.AddDays(3);
            DateTime futureDate2 = currentDate.AddDays(5);
            string temp = "Thời gian giao dự kiến từ " + futureDate.ToString("dd/MM/yyyy") + " đến " + futureDate2.ToString("dd/MM/yyyy");
            string productname = "";
            OrderGift OG = await _unitOfWork.GetRepository<OrderGift>().GetByIdAsync(id); // lấy thông tin theo id
            ApplicationUser user = await _unitOfWork.GetRepository<ApplicationUser>().GetByIdAsync(OG.UserID);
            int dem = 0;

            IEnumerable<OrderDetailGift> ODG = await _unitOfWork.GetRepository<OrderDetailGift>().GetAllAsync();
            foreach (var item in ODG)
            {

                if (item.OrderGiftId == id)
                {
                    Gift gift = await _unitOfWork.GetRepository<Gift>().GetByIdAsync(item.GiftId);
                    dem += gift.point * item.quantity;
                    productname += " " + gift.Products.ProductName + " Số lượng: " + item.quantity;
                }
            }
            string temp1 = " Quà tặng gồm: " + productname;
            if (OG.Status == "Confirmed" && OG.User != null)
            {
                _emailService.SendEmailAsync(user.Email, "ĐỔI ĐIỂM LẤY QUÀ - MILKSTORE", temp + temp1);
                user.Points = user.Points - dem;
                await _unitOfWork.GetRepository<ApplicationUser>().UpdateAsync(user);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}
