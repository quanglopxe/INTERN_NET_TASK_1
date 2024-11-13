using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Core.Utils;
using MilkStore.Repositories.Context;

namespace MilkStore.Services.Configs
{
    public static class SeedDataProduct
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            DatabaseContext? context = serviceProvider.GetRequiredService<DatabaseContext>();

            if (!context.Products.Any())
            {
                var categories = await context.Category.ToListAsync();

                var products = new List<Products>
                {
                    new Products
                    {
                        Id = Guid.NewGuid().ToString(),
                        ProductName = "Vinamilk Fresh Milk",
                        Price = 25000,
                        Description = "Sữa tươi tiệt trùng Vinamilk 1L",
                        QuantityInStock = 100,
                        ImageUrl = "vinamilk-fresh.jpg",
                        CategoryId = categories[0].Id,
                        CreatedTime = CoreHelper.SystemTimeNow
                    },
                    new Products
                    {
                        Id = Guid.NewGuid().ToString(),
                        ProductName = "Ensure Gold",
                        Price = 750000,
                        Description = "Sữa bột Ensure Gold 850g",
                        QuantityInStock = 50,
                        ImageUrl = "ensure-gold.jpg",
                        CategoryId = categories[1].Id,
                        CreatedTime = CoreHelper.SystemTimeNow
                    },
                    new Products
                    {
                        Id = Guid.NewGuid().ToString(),
                        ProductName = "TH True Yogurt",
                        Price = 30000,
                        Description = "Sữa chua TH True Milk lốc 4 hộp",
                        QuantityInStock = 200,
                        ImageUrl = "th-yogurt.jpg",
                        CategoryId = categories[2].Id,
                        CreatedTime = CoreHelper.SystemTimeNow
                    },
                    new Products
                    {
                        Id = Guid.NewGuid().ToString(),
                        ProductName = "Ông Thọ Đặc",
                        Price = 25000,
                        Description = "Sữa đặc Ông Thọ 380g",
                        QuantityInStock = 150,
                        ImageUrl = "ong-tho.jpg",
                        CategoryId = categories[3].Id,
                        CreatedTime = CoreHelper.SystemTimeNow
                    },
                    new Products
                    {
                        Id = Guid.NewGuid().ToString(),
                        ProductName = "Sữa Hạnh Nhân Mộc Châu",
                        Price = 35000,
                        Description = "Sữa hạnh nhân nguyên chất 1L",
                        QuantityInStock = 80,
                        ImageUrl = "hanh-nhan.jpg",
                        CategoryId = categories[4].Id,
                        CreatedTime = CoreHelper.SystemTimeNow
                    },
                    new Products
                    {
                        Id = Guid.NewGuid().ToString(),
                        ProductName = "Similac IQ 3",
                        Price = 485000,
                        Description = "Sữa bột công thức Similac IQ 3 900g",
                        QuantityInStock = 60,
                        ImageUrl = "similac.jpg",
                        CategoryId = categories[5].Id,
                        CreatedTime = CoreHelper.SystemTimeNow
                    },
                    new Products
                    {
                        Id = Guid.NewGuid().ToString(),
                        ProductName = "Vinamilk Organic Gold",
                        Price = 195000,
                        Description = "Sữa bột organic cho trẻ 1-2 tuổi",
                        QuantityInStock = 75,
                        ImageUrl = "organic-gold.jpg",
                        CategoryId = categories[6].Id,
                        CreatedTime = CoreHelper.SystemTimeNow
                    },
                    new Products
                    {
                        Id = Guid.NewGuid().ToString(),
                        ProductName = "TH True Milk No Sugar",
                        Price = 28000,
                        Description = "Sữa tươi không đường 1L",
                        QuantityInStock = 90,
                        ImageUrl = "th-no-sugar.jpg",
                        CategoryId = categories[7].Id,
                        CreatedTime = CoreHelper.SystemTimeNow
                    },
                    new Products
                    {
                        Id = Guid.NewGuid().ToString(),
                        ProductName = "Vinamilk Less Sugar",
                        Price = 26000,
                        Description = "Sữa tươi ít đường 1L",
                        QuantityInStock = 85,
                        ImageUrl = "less-sugar.jpg",
                        CategoryId = categories[8].Id,
                        CreatedTime = CoreHelper.SystemTimeNow
                    },
                    new Products
                    {
                        Id = Guid.NewGuid().ToString(),
                        ProductName = "Glucerna",
                        Price = 820000,
                        Description = "Sữa cho người tiểu đường 850g",
                        QuantityInStock = 40,
                        ImageUrl = "glucerna.jpg",
                        CategoryId = categories[9].Id,
                        CreatedTime = CoreHelper.SystemTimeNow
                    }
                };

                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
            }
        }
    }
}