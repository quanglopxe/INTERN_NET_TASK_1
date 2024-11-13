using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Core.Utils;
using MilkStore.Repositories.Context;
using MilkStore.Repositories.Entity;

namespace MilkStore.Services.Configs
{
    public static class SeedDataTransaction
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            DatabaseContext? context = serviceProvider.GetRequiredService<DatabaseContext>();

            if (!context.TransactionHistories.Any())
            {
                List<ApplicationUser>? users = await context.ApplicationUsers.Take(3).ToListAsync();
                List<TransactionHistory>? transactions = new List<TransactionHistory>
                {
                    new TransactionHistory
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = users[0].Id,
                        TransactionDate = DateTime.UtcNow.AddDays(-10),
                        Type = TransactionType.UserWallet,
                        Amount = 1000000,
                        BalanceAfterTransaction = 1000000,
                        Content = "Nạp tiền vào ví",
                        CreatedTime = DateTime.UtcNow.AddDays(-10)
                    },
                    new TransactionHistory
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = users[1].Id,
                        TransactionDate = DateTime.UtcNow.AddDays(-9),
                        Type = TransactionType.UserWallet,
                        Amount = 2000000,
                        BalanceAfterTransaction = 2000000,
                        Content = "Nạp tiền vào ví",
                        CreatedTime = DateTime.UtcNow.AddDays(-9)
                    },
                    new TransactionHistory
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = users[2].Id,
                        TransactionDate = DateTime.UtcNow.AddDays(-8),
                        Type = TransactionType.Vnpay,
                        Amount = -500000,
                        BalanceAfterTransaction = 1500000,
                        Content = "Thanh toán đơn hàng #123",
                        CreatedTime = DateTime.UtcNow.AddDays(-8)
                    },
                    new TransactionHistory
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = users[0].Id,
                        TransactionDate = DateTime.UtcNow.AddDays(-7),
                        Type = TransactionType.UserWallet,
                        Amount = 200000,
                        BalanceAfterTransaction = 1200000,
                        Content = "Hoàn tiền đơn hàng #124",
                        CreatedTime = DateTime.UtcNow.AddDays(-7)
                    },
                    new TransactionHistory
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = users[1].Id,
                        TransactionDate = DateTime.UtcNow.AddDays(-6),
                        Type = TransactionType.Vnpay,
                        Amount = -300000,
                        BalanceAfterTransaction = 1700000,
                        Content = "Thanh toán đơn hàng #125",
                        CreatedTime = DateTime.UtcNow.AddDays(-6)
                    },
                    new TransactionHistory
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = users[2].Id,
                        TransactionDate = DateTime.UtcNow.AddDays(-5),
                        Type = TransactionType.UserWallet,
                        Amount = 1500000,
                        BalanceAfterTransaction = 3000000,
                        Content = "Nạp tiền vào ví",
                        CreatedTime = DateTime.UtcNow.AddDays(-5)
                    },
                    new TransactionHistory
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = users[0].Id,
                        TransactionDate = DateTime.UtcNow.AddDays(-4),
                        Type = TransactionType.Vnpay,
                        Amount = -800000,
                        BalanceAfterTransaction = 400000,
                        Content = "Thanh toán đơn hàng #126",
                        CreatedTime = DateTime.UtcNow.AddDays(-4)
                    },
                    new TransactionHistory
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = users[1].Id,
                        TransactionDate = DateTime.UtcNow.AddDays(-3),
                        Type = TransactionType.UserWallet,
                        Amount = 150000,
                        BalanceAfterTransaction = 1850000,
                        Content = "Hoàn tiền đơn hàng #127",
                        CreatedTime = DateTime.UtcNow.AddDays(-3)
                    },
                    new TransactionHistory
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = users[2].Id,
                        TransactionDate = DateTime.UtcNow.AddDays(-2),
                        Type = TransactionType.Vnpay,
                        Amount = -1000000,
                        BalanceAfterTransaction = 2000000,
                        Content = "Thanh toán đơn hàng #128",
                        CreatedTime = DateTime.UtcNow.AddDays(-2)
                    },
                    new TransactionHistory
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = users[0].Id,
                        TransactionDate = DateTime.UtcNow.AddDays(-1),
                        Type = TransactionType.UserWallet,
                        Amount = 500000,
                        BalanceAfterTransaction = 900000,
                        Content = "Nạp tiền vào ví",
                        CreatedTime = DateTime.UtcNow.AddDays(-1)
                    }
                };

                await context.TransactionHistories.AddRangeAsync(transactions);
                await context.SaveChangesAsync();
            }
        }
    }
}