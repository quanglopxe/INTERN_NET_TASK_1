﻿using Microsoft.AspNetCore.Identity;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Core;
using MilkStore.ModelViews.ResponseDTO;
using MilkStore.ModelViews.VoucherModelViews;
using MilkStore.Repositories.Entity;

namespace MilkStore.Contract.Services.Interface
{
    public interface IVoucherService
    {
        Task<BasePaginatedList<VoucherResponseDTO>> GetVouchers(string? name, int pageIndex, int pageSize);
        Task CreateVoucher(VoucherModelView voucherModel);
        Task UpdateVoucher(string id, VoucherModelView voucherModel);
        Task DeleteVoucher(string id);
    }
}
