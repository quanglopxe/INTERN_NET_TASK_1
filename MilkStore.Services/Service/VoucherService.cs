using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Repositories.Interface;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core.Base;
using MilkStore.Core.Constants;
using MilkStore.Core.Utils;
using MilkStore.ModelViews.AuthModelViews;
using MilkStore.ModelViews.VoucherModelViews;
using MilkStore.ModelViews.UserModelViews;
using MilkStore.Repositories.Context;
using MilkStore.Repositories.Entity;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace MilkStore.Services.Service
{
    public class VoucherService : IVoucherService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DatabaseContext context;
        public VoucherService(DatabaseContext context, IUnitOfWork unitOfWork)
        {
            this.context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<Voucher> CreateVoucher(VoucherModelView voucherModel)
        {
            var newVoucher = new Voucher
            {
                Description = voucherModel.Description,
                SalePrice = voucherModel.SalePrice,
                SalePercent = voucherModel.SalePercent,
                LimitSalePrice = voucherModel.LimitSalePrice,
                ExpiryDate = voucherModel.ExpiryDate,
                UsingLimit = voucherModel.UsingLimit,
                UsedCount = voucherModel.UsedCount,
                Status = voucherModel.Status,
                Name = voucherModel.Name,
                CreatedTime = CoreHelper.SystemTimeNow,
                LastUpdatedTime = CoreHelper.SystemTimeNow,
                DeletedTime = null
            };
            await _unitOfWork.GetRepository<Voucher>().InsertAsync(newVoucher);
            await _unitOfWork.SaveAsync();
            return newVoucher;
        }

        public async Task DeleteVoucher(string id)
        {
            var voucher = await _unitOfWork.GetRepository<Voucher>().GetByIdAsync(id);
            if (voucher == null)
            {
                throw new KeyNotFoundException($"Voucher have ID: {id} was not found.");
            }
            voucher.DeletedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<Voucher>().UpdateAsync(voucher);
            await _unitOfWork.SaveAsync();

        }

        public async Task<IEnumerable<Voucher>> GetVouchers(string? id)
        {
            if (id == null)
            {
                return await _unitOfWork.GetRepository<Voucher>().Entities.Where(voucher => voucher.DeletedTime == null).ToListAsync();
            }
            else
            {
                var voucher = await _unitOfWork.GetRepository<Voucher>().Entities.FirstOrDefaultAsync(voucher => voucher.Id == id && voucher.DeletedTime == null);
                if (voucher == null)
                {
                    throw new KeyNotFoundException($"Voucher have ID: {id} was not found.");
                }
                return new List<Voucher> { voucher };
            }

        }

        public async Task<Voucher> UpdateVoucher(string id, VoucherModelView voucherModel)
        {
            var voucher = await _unitOfWork.GetRepository<Voucher>().GetByIdAsync(id);
            if (voucher == null)
            {
                throw new KeyNotFoundException($"Voucher have ID: {id} was not found.");
            }
            voucher.Description = voucherModel.Description;
            voucher.SalePrice = voucherModel.SalePrice;
            voucher.SalePercent = voucherModel.SalePercent;
            voucher.LimitSalePrice = voucherModel.LimitSalePrice;
            voucher.ExpiryDate = voucherModel.ExpiryDate;
            voucher.UsingLimit = voucherModel.UsingLimit;
            voucher.UsedCount = voucherModel.UsedCount;
            voucher.Status = voucherModel.Status;
            voucher.Name = voucherModel.Name;
            voucher.CreatedTime = CoreHelper.SystemTimeNow;
            voucher.LastUpdatedTime = CoreHelper.SystemTimeNow;
            voucher.DeletedTime = voucherModel.DeletedTime;

            await _unitOfWork.GetRepository<Voucher>().UpdateAsync(voucher);
            await _unitOfWork.SaveAsync();
            return voucher;
        }
    }
}
