using Microsoft.Extensions.DependencyInjection;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Core.Utils;
using MilkStore.Repositories.Context;

namespace MilkStore.Services.Configs
{
    public static class SeedDataVoucher
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            DatabaseContext? context = serviceProvider.GetRequiredService<DatabaseContext>();

            if (!context.Vouchers.Any())
            {
                List<Voucher>? vouchers = new List<Voucher>
                {
                    new Voucher
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Khuyến mãi mùa hè",
                        Description = "Giảm 20% tối đa 100k",
                        SalePercent = 20,
                        SalePrice = 0,
                        LimitSalePrice = 100000,
                        ExpiryDate = CoreHelper.SystemTimeNow.DateTime.AddMonths(1),
                        UsingLimit = 100,
                        UsedCount = 0,
                        Status = 1,
                        VoucherCode = "SUMMER23",
                        CreatedTime = CoreHelper.SystemTimeNow
                    },
                    new Voucher
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Giảm giá cố định",
                        Description = "Giảm 50k cho đơn từ 300k",
                        SalePercent = 0,
                        SalePrice = 50000,
                        LimitSalePrice = 50000,
                        ExpiryDate = CoreHelper.SystemTimeNow.DateTime.AddMonths(1),
                        UsingLimit = 50,
                        UsedCount = 0,
                        Status = 1,
                        VoucherCode = "FIXED50",
                        CreatedTime = CoreHelper.SystemTimeNow
                    },
                    new Voucher
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Mã giảm giá mới",
                        Description = "Giảm 15% tối đa 30k cho đơn hàng đầu tiên",
                        SalePercent = 15,
                        SalePrice = 0,
                        LimitSalePrice = 30000,
                        ExpiryDate = CoreHelper.SystemTimeNow.DateTime.AddMonths(2),
                        UsingLimit = 200,
                        UsedCount = 0,
                        Status = 1,
                        VoucherCode = "NEWUSER",
                        CreatedTime = CoreHelper.SystemTimeNow
                    },
                    new Voucher
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Ưu đãi cuối tuần",
                        Description = "Giảm 10% không giới hạn",
                        SalePercent = 10,
                        SalePrice = 0,
                        LimitSalePrice = 1000000,
                        ExpiryDate = CoreHelper.SystemTimeNow.DateTime.AddDays(7),
                        UsingLimit = 100,
                        UsedCount = 0,
                        Status = 1,
                        VoucherCode = "WEEKEND",
                        CreatedTime = CoreHelper.SystemTimeNow
                    },
                    new Voucher
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Sinh nhật MilkStore",
                        Description = "Giảm 100k cho đơn từ 500k",
                        SalePercent = 0,
                        SalePrice = 100000,
                        LimitSalePrice = 100000,
                        ExpiryDate = CoreHelper.SystemTimeNow.DateTime.AddDays(3),
                        UsingLimit = 50,
                        UsedCount = 0,
                        Status = 1,
                        VoucherCode = "BIRTH23",
                        CreatedTime = CoreHelper.SystemTimeNow
                    },
                    new Voucher
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Mua nhiều giảm nhiều",
                        Description = "Giảm 25% tối đa 200k cho đơn từ 1000k",
                        SalePercent = 25,
                        SalePrice = 0,
                        LimitSalePrice = 200000,
                        ExpiryDate = CoreHelper.SystemTimeNow.DateTime.AddMonths(1),
                        UsingLimit = 30,
                        UsedCount = 0,
                        Status = 1,
                        VoucherCode = "BULK25",
                        CreatedTime = CoreHelper.SystemTimeNow
                    },
                    new Voucher
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Flash Sale",
                        Description = "Giảm 40% tối đa 100k trong 2 giờ",
                        SalePercent = 40,
                        SalePrice = 0,
                        LimitSalePrice = 100000,
                        ExpiryDate = CoreHelper.SystemTimeNow.DateTime.AddHours(2),
                        UsingLimit = 20,
                        UsedCount = 0,
                        Status = 1,
                        VoucherCode = "FLASH40",
                        CreatedTime = CoreHelper.SystemTimeNow
                    },
                    new Voucher
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Khách hàng thân thiết",
                        Description = "Giảm 30k không điều kiện",
                        SalePercent = 0,
                        SalePrice = 30000,
                        LimitSalePrice = 30000,
                        ExpiryDate = CoreHelper.SystemTimeNow.DateTime.AddMonths(3),
                        UsingLimit = 1,
                        UsedCount = 0,
                        Status = 1,
                        VoucherCode = "VIP30K",
                        CreatedTime = CoreHelper.SystemTimeNow
                    },
                    new Voucher
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Combo tiết kiệm",
                        Description = "Giảm 70k cho đơn từ 400k",
                        SalePercent = 0,
                        SalePrice = 70000,
                        LimitSalePrice = 70000,
                        ExpiryDate = CoreHelper.SystemTimeNow.DateTime.AddMonths(1),
                        UsingLimit = 40,
                        UsedCount = 0,
                        Status = 1,
                        VoucherCode = "COMBO70",
                        CreatedTime = CoreHelper.SystemTimeNow
                    },
                    new Voucher
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Ưu đãi thanh toán",
                        Description = "Giảm 5% tối đa 50k khi thanh toán online",
                        SalePercent = 5,
                        SalePrice = 0,
                        LimitSalePrice = 50000,
                        ExpiryDate = CoreHelper.SystemTimeNow.DateTime.AddMonths(1),
                        UsingLimit = 100,
                        UsedCount = 0,
                        Status = 1,
                        VoucherCode = "ONLINE5",
                        CreatedTime = CoreHelper.SystemTimeNow
                    }
                };

                await context.Vouchers.AddRangeAsync(vouchers);
                await context.SaveChangesAsync();
            }
        }
    }
}