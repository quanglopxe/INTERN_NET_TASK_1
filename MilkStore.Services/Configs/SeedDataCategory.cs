using Microsoft.Extensions.DependencyInjection;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Core.Utils;
using MilkStore.Repositories.Context;

namespace MilkStore.Services.Configs
{
    public static class SeedDataCategory
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            DatabaseContext? context = serviceProvider.GetRequiredService<DatabaseContext>();

            if (!context.Category.Any())
            {
                var categories = new List<Category>
                {
                    new Category
                    {
                        Id = Guid.NewGuid().ToString(),
                        CategoryName = "Sữa tươi",
                        CreatedTime = CoreHelper.SystemTimeNow
                    },
                    new Category
                    {
                        Id = Guid.NewGuid().ToString(),
                        CategoryName = "Sữa bột",
                        CreatedTime = CoreHelper.SystemTimeNow
                    },
                    new Category
                    {
                        Id = Guid.NewGuid().ToString(),
                        CategoryName = "Sữa chua",
                        CreatedTime = CoreHelper.SystemTimeNow
                    },
                    new Category
                    {
                        Id = Guid.NewGuid().ToString(),
                        CategoryName = "Sữa đặc",
                        CreatedTime = CoreHelper.SystemTimeNow
                    },
                    new Category
                    {
                        Id = Guid.NewGuid().ToString(),
                        CategoryName = "Sữa hạt",
                        CreatedTime = CoreHelper.SystemTimeNow
                    },
                    new Category
                    {
                        Id = Guid.NewGuid().ToString(),
                        CategoryName = "Sữa công thức",
                        CreatedTime = CoreHelper.SystemTimeNow
                    },
                    new Category
                    {
                        Id = Guid.NewGuid().ToString(),
                        CategoryName = "Sữa organic",
                        CreatedTime = CoreHelper.SystemTimeNow
                    },
                    new Category
                    {
                        Id = Guid.NewGuid().ToString(),
                        CategoryName = "Sữa không đường",
                        CreatedTime = CoreHelper.SystemTimeNow
                    },
                    new Category
                    {
                        Id = Guid.NewGuid().ToString(),
                        CategoryName = "Sữa ít đường",
                        CreatedTime = CoreHelper.SystemTimeNow
                    },
                    new Category
                    {
                        Id = Guid.NewGuid().ToString(),
                        CategoryName = "Sữa cho người tiểu đường",
                        CreatedTime = CoreHelper.SystemTimeNow
                    }
                };

                await context.Category.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }
        }
    }
}