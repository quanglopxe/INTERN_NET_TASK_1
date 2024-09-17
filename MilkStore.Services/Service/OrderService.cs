using MilkStore.Contract.Services.Interface;
using MilkStore.ModelViews.OrderModelViews;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Repositories.Interface;
using MilkStore.Core.Utils;
using MilkStore.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using MilkStore.ModelViews.ResponseDTO;
using AutoMapper;
using MilkStore.Repositories.Entity;

namespace MilkStore.Services.Service
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DatabaseContext _context;
        protected readonly DbSet<Order> _dbSet;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, DatabaseContext context, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _dbSet = _context.Set<Order>();
            _mapper = mapper;
        }

        private OrderResponseDTO MapToOrderResponseDto(Order order)
        {
            return _mapper.Map<OrderResponseDTO>(order);
        }

        public async Task<IEnumerable<OrderResponseDTO>> GetAsync(string? id)
        {
            if(string.IsNullOrWhiteSpace(id))
            {
                List<Order> listOrder = await _dbSet.Where
                    (e => !EF.Property<DateTimeOffset?>(e, "DeletedTime").HasValue)
                    .ToListAsync();
                return listOrder.Select(MapToOrderResponseDto).ToList();
            }
            else
            {
                Order ord = await _unitOfWork.GetRepository<Order>().Entities
                    .FirstOrDefaultAsync(or => or.Id == id && !or.DeletedTime.HasValue);
                if(ord is null)
                {
                    return null;
                }
                return new List<OrderResponseDTO> { MapToOrderResponseDto(ord) };
            }
        }

        public async Task<Order> AddAsync(OrderModelView ord)
        {
            // Sử dụng mapper để ánh xạ từ OrderModelView sang Order
            Order item = _mapper.Map<Order>(ord);

            // Đảm bảo gán các giá trị khác không được ánh xạ từ model view
            item.TotalAmount = 0;
            item.DiscountedAmount = 0;

            // Kiểm tra sự tồn tại của User
            if (await _unitOfWork.GetRepository<ApplicationUser>().Entities
                    .AnyAsync(u => u.Id == ord.UserId) == false)
            {
                throw new KeyNotFoundException($"User with ID {ord.UserId} does not exist.");
            }

            // Kiểm tra sự tồn tại của Voucher
            if (ord.VoucherId is not null)
            {
                Voucher vch = await _unitOfWork.GetRepository<Voucher>().Entities
                .FirstOrDefaultAsync(v => v.Id == ord.VoucherId && !v.DeletedTime.HasValue);
                if (vch is not null)
                {
                    vch.UsedCount++;
                    await _unitOfWork.GetRepository<Voucher>().UpdateAsync(vch);
                }
                else
                {
                    throw new KeyNotFoundException($"Voucher with ID {ord.VoucherId} does not exist.");
                }
            }

            await _unitOfWork.GetRepository<Order>().InsertAsync(item);
            await _unitOfWork.SaveAsync();
            return item;
        }

        public async Task<Order> UpdateAsync(string id, OrderModelView ord)
        {
            // Lấy đối tượng hiện tại từ cơ sở dữ liệu
            Order orderss = await _unitOfWork.GetRepository<Order>().Entities
                .FirstOrDefaultAsync(or => or.Id == id && !or.DeletedTime.HasValue)
                ?? throw new KeyNotFoundException($"Order with ID {id} was not found or is deleted.");

            // Sử dụng AutoMapper để ánh xạ những thay đổi
            _mapper.Map(ord, orderss);  // Chỉ ánh xạ những thuộc tính có giá trị khác biệt

            if (await _unitOfWork.GetRepository<ApplicationUser>().Entities
                    .AnyAsync(u => u.Id == ord.UserId) == false)
            {
                throw new KeyNotFoundException($"User with ID {ord.UserId} does not exist.");
            }

            // Kiểm tra sự tồn tại của Voucher
            if (ord.VoucherId is not null && await _unitOfWork.GetRepository<Voucher>().Entities
                    .AnyAsync(v => v.Id == ord.VoucherId) == false)
            {
                throw new KeyNotFoundException($"Voucher with ID {ord.VoucherId} does not exist.");
            }

            // Cập nhật thời gian cập nhật
            orderss.LastUpdatedTime = CoreHelper.SystemTimeNow;

            // Lưu thay đổi vào cơ sở dữ liệu
            await _unitOfWork.GetRepository<Order>().UpdateAsync(orderss);
            await _unitOfWork.SaveAsync();

            return orderss;
        }

        //Cập nhật TotalAmount
        public async Task UpdateToTalAmount (string id)
        {

            Order ord = await _unitOfWork.GetRepository<Order>().Entities
                .FirstOrDefaultAsync(or => or.Id == id && !or.DeletedTime.HasValue)
                ?? throw new KeyNotFoundException($"Order with ID {id} was not found or is deleted.");
            List<OrderDetails> lstOrd = await _unitOfWork.GetRepository<OrderDetails>().Entities
                .Where(or => or.OrderID == id).ToListAsync();
            ord.TotalAmount = lstOrd.Sum(o => o.TotalAmount);

            double discountAmount = 0;
            //Tính thành tiền áp dụng ưu đãi
            Voucher vch = await _unitOfWork.GetRepository<Voucher>().Entities
                .FirstOrDefaultAsync(v => v.Id == ord.VoucherId && !v.DeletedTime.HasValue);
            if (vch is { ExpiryDate: var expiryDate, 
                LimitSalePrice: var limitSalePrice, 
                SalePercent: var salePercent, 
                UsedCount: var usedCount, 
                UsingLimit: var usingLimit })
            {
                if (expiryDate > ord.OrderDate
                    && Convert.ToDouble(limitSalePrice) <= ord.TotalAmount
                    && usedCount < usingLimit)
                {
                    discountAmount = (ord.TotalAmount * salePercent) / 100.0;
                }
            }
            ord.DiscountedAmount = ord.TotalAmount - discountAmount;
            await _unitOfWork.GetRepository<Order>().UpdateAsync(ord);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(string id)
        {
            Order orderss = await _unitOfWork.GetRepository<Order>().Entities
               .FirstOrDefaultAsync(or => or.Id == id && !or.DeletedTime.HasValue)
               ?? throw new KeyNotFoundException($"Order with ID {id} was not found or is deleted.");
            orderss.DeletedTime = CoreHelper.SystemTimeNow;
            await _unitOfWork.GetRepository<Order>().UpdateAsync(orderss);
            await _unitOfWork.SaveAsync();
        }
    }
}
