using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Repositories.Interface;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core;
using MilkStore.Core.Utils;
using MilkStore.ModelViews.VoucherModelViews;
using MilkStore.ModelViews.ResponseDTO;


namespace MilkStore.Services.Service
{
    public class VoucherService : IVoucherService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public VoucherService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task CreateVoucher(VoucherModelView voucherModel)
        {
            if (voucherModel == null)
            {
                throw new ArgumentNullException(nameof(voucherModel), "Voucher model cannot be null.");
            }

            Voucher newVoucher = _mapper.Map<Voucher>(voucherModel);
            newVoucher.CreatedTime = CoreHelper.SystemTimeNow;
            newVoucher.LastUpdatedTime = CoreHelper.SystemTimeNow;
            newVoucher.DeletedTime = null;

            await _unitOfWork.GetRepository<Voucher>().InsertAsync(newVoucher);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteVoucher(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new KeyNotFoundException("Voucher ID is required.");
            }

            Voucher voucher = await _unitOfWork.GetRepository<Voucher>().GetByIdAsync(id);
            if (voucher == null)
            {
                throw new KeyNotFoundException($"Voucher with ID {id} was not found.");
            }

            if (voucher.DeletedTime != null)
            {
                throw new InvalidOperationException($"Voucher with ID {id} has already been deleted.");
            }

            voucher.DeletedTime = CoreHelper.SystemTimeNow;
            await _unitOfWork.GetRepository<Voucher>().UpdateAsync(voucher);
            await _unitOfWork.SaveAsync();
        }

        public async Task<BasePaginatedList<VoucherResponseDTO>> GetVouchers(string? name, int pageIndex, int pageSize)
        {
            var query = _unitOfWork.GetRepository<Voucher>().Entities.Where(voucher => voucher.DeletedTime == null);

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(voucher => voucher.Name.Contains(name));
            }

            var paginatedVouchers = await _unitOfWork.GetRepository<Voucher>().GetPagging(query, pageIndex, pageSize);

            if (!paginatedVouchers.Items.Any())
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    var vouchersByName = await _unitOfWork.GetRepository<Voucher>().Entities
                        .Where(voucher => voucher.Name.Contains(name) && voucher.DeletedTime == null)
                        .ToListAsync();

                    if (vouchersByName.Any())
                    {
                        var paginatedVoucherDtos = _mapper.Map<List<VoucherResponseDTO>>(vouchersByName);
                        return new BasePaginatedList<VoucherResponseDTO>(paginatedVoucherDtos, vouchersByName.Count, 1, pageSize);
                    }
                }
            }

            var voucherDtosResult = _mapper.Map<List<VoucherResponseDTO>>(paginatedVouchers.Items);

            return new BasePaginatedList<VoucherResponseDTO>(
                voucherDtosResult,
                paginatedVouchers.TotalItems,
                paginatedVouchers.CurrentPage,
                paginatedVouchers.PageSize
            );
        }

        public async Task UpdateVoucher(string id, VoucherModelView voucherModel)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new KeyNotFoundException("Voucher ID is required.");
            }

            Voucher voucher = await _unitOfWork.GetRepository<Voucher>().GetByIdAsync(id);
            if (voucher == null)
            {
                throw new KeyNotFoundException($"Voucher with ID {id} was not found.");
            }

            // Map voucherModel to voucher (only update changed fields)
            _mapper.Map(voucherModel, voucher);
            voucher.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<Voucher>().UpdateAsync(voucher);
            await _unitOfWork.SaveAsync();
        }
    }
}
