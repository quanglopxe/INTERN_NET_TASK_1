using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Core.Utils;
using MilkStore.Repositories.Context;

namespace MilkStore.Services.Configs
{
    public static class SeedDataGift
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            DatabaseContext? context = serviceProvider.GetRequiredService<DatabaseContext>();

            if (!context.Gifts.Any())
            {
                List<Products>? products = await context.Products.Take(5).ToListAsync();

                List<Gift>? gifts = new List<Gift>
                {
                    new Gift
                    {
                        Id = Guid.NewGuid().ToString(),
                        GiftName = "Quà tặng sinh nhật",
                        point = 100,
                        ProductId = products[0].Id,
                        CreatedTime = DateTime.UtcNow
                    },
                    new Gift
                    {
                        Id = Guid.NewGuid().ToString(),
                        GiftName = "Quà tặng khách VIP",
                        point = 200,
                        ProductId = products[1].Id,
                        CreatedTime = DateTime.UtcNow
                    },
                    new Gift
                    {
                        Id = Guid.NewGuid().ToString(),
                        GiftName = "Quà tặng năm mới",
                        point = 150,
                        ProductId = products[2].Id,
                        CreatedTime = DateTime.UtcNow
                    },
                    new Gift
                    {
                        Id = Guid.NewGuid().ToString(),
                        GiftName = "Quà tặng khách hàng thân thiết",
                        point = 300,
                        ProductId = products[3].Id,
                        CreatedTime = DateTime.UtcNow
                    },
                    new Gift
                    {
                        Id = Guid.NewGuid().ToString(),
                        GiftName = "Quà tặng mùa hè",
                        point = 120,
                        ProductId = products[4].Id,
                        CreatedTime = DateTime.UtcNow
                    },
                    new Gift
                    {
                        Id = Guid.NewGuid().ToString(),
                        GiftName = "Quà tặng tri ân",
                        point = 250,
                        ProductId = products[0].Id,
                        CreatedTime = DateTime.UtcNow
                    },
                    new Gift
                    {
                        Id = Guid.NewGuid().ToString(),
                        GiftName = "Quà tặng tết",
                        point = 400,
                        ProductId = products[1].Id,
                        CreatedTime = DateTime.UtcNow
                    },
                    new Gift
                    {
                        Id = Guid.NewGuid().ToString(),
                        GiftName = "Quà tặng khai trương",
                        point = 180,
                        ProductId = products[2].Id,
                        CreatedTime = DateTime.UtcNow
                    },
                    new Gift
                    {
                        Id = Guid.NewGuid().ToString(),
                        GiftName = "Quà tặng giáng sinh",
                        point = 350,
                        ProductId = products[3].Id,
                        CreatedTime = DateTime.UtcNow
                    },
                    new Gift
                    {
                        Id = Guid.NewGuid().ToString(),
                        GiftName = "Quà tặng đặc biệt",
                        point = 500,
                        ProductId = products[4].Id,
                        CreatedTime = DateTime.UtcNow
                    }
                };

                await context.Gifts.AddRangeAsync(gifts);
                await context.SaveChangesAsync();
            }
        }
    }
}