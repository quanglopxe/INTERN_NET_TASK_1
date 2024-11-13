using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Core.Utils;
using MilkStore.Repositories.Context;
using MilkStore.Repositories.Entity;

namespace MilkStore.Services.Configs
{
    public static class SeedDataPreOrder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            DatabaseContext? context = serviceProvider.GetRequiredService<DatabaseContext>();

            if (!context.PreOrders.Any())
            {
                List<ApplicationUser>? users = await context.ApplicationUsers.Take(3).ToListAsync();
                List<Products>? products = await context.Products.Take(4).ToListAsync();

                List<PreOrders>? preOrders = new List<PreOrders>
                {
                    new PreOrders
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserID = users[0].Id,
                        ProductID = products[0].Id,
                        Status = PreOrderStatus.Pending,
                        CreatedTime = DateTime.UtcNow
                    },
                    new PreOrders
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserID = users[1].Id,
                        ProductID = products[1].Id,
                        Status = PreOrderStatus.Available,
                        CreatedTime = DateTime.UtcNow
                    },
                    new PreOrders
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserID = users[2].Id,
                        ProductID = products[2].Id,
                        Status = PreOrderStatus.Confirmed,
                        CreatedTime = DateTime.UtcNow
                    },
                    new PreOrders
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserID = users[0].Id,
                        ProductID = products[3].Id,
                        Status = PreOrderStatus.Pending,
                        CreatedTime = DateTime.UtcNow
                    },
                    new PreOrders
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserID = users[1].Id,
                        ProductID = products[0].Id,
                        Status = PreOrderStatus.Available,
                        CreatedTime = DateTime.UtcNow
                    },
                    new PreOrders
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserID = users[2].Id,
                        ProductID = products[1].Id,
                        Status = PreOrderStatus.Pending,
                        CreatedTime = DateTime.UtcNow
                    },
                    new PreOrders
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserID = users[0].Id,
                        ProductID = products[2].Id,
                        Status = PreOrderStatus.Confirmed,
                        CreatedTime = DateTime.UtcNow
                    },
                    new PreOrders
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserID = users[1].Id,
                        ProductID = products[3].Id,
                        Status = PreOrderStatus.Available,
                        CreatedTime = DateTime.UtcNow
                    },
                    new PreOrders
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserID = users[2].Id,
                        ProductID = products[0].Id,
                        Status = PreOrderStatus.Pending,
                        CreatedTime = DateTime.UtcNow
                    },
                    new PreOrders
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserID = users[0].Id,
                        ProductID = products[1].Id,
                        Status = PreOrderStatus.Confirmed,
                        CreatedTime = DateTime.UtcNow
                    }
                };

                await context.PreOrders.AddRangeAsync(preOrders);
                await context.SaveChangesAsync();
            }
        }
    }
}