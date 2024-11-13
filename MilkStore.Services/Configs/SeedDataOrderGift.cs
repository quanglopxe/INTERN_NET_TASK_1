using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Repositories.Context;
using MilkStore.Repositories.Entity;
namespace MilkStore.Services.Configs
{
    public class SeedDataOrderGift
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            DatabaseContext? context = serviceProvider.GetRequiredService<DatabaseContext>();

            if (!context.OrderGifts.Any())
            {
                List<ApplicationUser>? users = await context.ApplicationUsers.Take(3).ToListAsync();
                List<Gift>? gifts = await context.Gifts.Take(5).ToListAsync();

                List<OrderGift>? orderGifts = new List<OrderGift>
                {
                    new OrderGift
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserID = users[0].Id,
                        Status = OrderGiftStatus.Pending,
                        Address = "123 Nguyen Van Linh, Da Nang",
                        CreatedTime = DateTime.UtcNow.AddDays(-10)
                    },
                    new OrderGift
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserID = users[1].Id,
                        Status = OrderGiftStatus.Confirmed,
                        Address = "123 Nguyen Van Linh, Da Nang",
                        CreatedTime = DateTime.UtcNow.AddDays(-9)
                    },
                    new OrderGift
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserID = users[2].Id,
                        Status = OrderGiftStatus.Cancelled,
                        Address = "123 Nguyen Van Linh, Da Nang",
                        CreatedTime = DateTime.UtcNow.AddDays(-8)
                    },
                    new OrderGift
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserID = users[0].Id,
                        Status = OrderGiftStatus.Pending,
                        Address = "123 Nguyen Van Linh, Da Nang",
                        CreatedTime = DateTime.UtcNow.AddDays(-7)
                    },
                    new OrderGift
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserID = users[1].Id,
                        Status = OrderGiftStatus.Confirmed,
                        Address = "123 Nguyen Van Linh, Da Nang",
                        CreatedTime = DateTime.UtcNow.AddDays(-6)
                    },
                    new OrderGift
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserID = users[2].Id,
                        Status = OrderGiftStatus.Delivered,
                        Address = "123 Nguyen Van Linh, Da Nang",
                        CreatedTime = DateTime.UtcNow.AddDays(-5)
                    },
                    new OrderGift
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserID = users[0].Id,
                        Status = OrderGiftStatus.Cancelled,
                        Address = "123 Nguyen Van Linh, Da Nang",
                        CreatedTime = DateTime.UtcNow.AddDays(-4)
                    },
                    new OrderGift
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserID = users[1].Id,
                        Status = OrderGiftStatus.Delivered,
                        Address = "123 Nguyen Van Linh, Da Nang",
                        CreatedTime = DateTime.UtcNow.AddDays(-3)
                    },
                    new OrderGift
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserID = users[2].Id,
                        Status = OrderGiftStatus.Pending,
                        Address = "123 Nguyen Van Linh, Da Nang",
                        CreatedTime = DateTime.UtcNow.AddDays(-2)
                    },
                    new OrderGift
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserID = users[0].Id,
                        Status = OrderGiftStatus.Confirmed,
                        Address = "123 Nguyen Van Linh, Da Nang",
                        CreatedTime = DateTime.UtcNow.AddDays(-1)
                    }
                };

                await context.OrderGifts.AddRangeAsync(orderGifts);
                await context.SaveChangesAsync();
            }
        }
    }
}