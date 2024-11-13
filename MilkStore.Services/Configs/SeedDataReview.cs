using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Core.Utils;
using MilkStore.Repositories.Context;
using MilkStore.Repositories.Entity;

namespace MilkStore.Services.Configs
{
    public static class SeedDataReview
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            DatabaseContext? context = serviceProvider.GetRequiredService<DatabaseContext>();

            if (!context.Reviews.Any())
            {
                List<ApplicationUser>? users = await context.ApplicationUsers.Take(3).ToListAsync();
                List<OrderDetails>? orderDetails = await context.OrderDetails
                    .Include(od => od.Order)
                    .Where(od => od.Status == OrderDetailStatus.Ordered)
                    .Take(10)
                    .ToListAsync();

                var reviews = new List<Review>
                {
                    new Review
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserID = users[0].Id,
                        OrderDetailID = orderDetails[0].Id,
                        ProductsID = orderDetails[0].ProductID,
                        OrderID = orderDetails[0].OrderID,
                        Rating = 5,
                        Comment = "Sản phẩm rất tốt, đóng gói cẩn thận",
                        CreatedTime = DateTime.UtcNow.AddDays(-10)
                    },
                    new Review
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserID = users[1].Id,
                        OrderDetailID = orderDetails[1].Id,
                        ProductsID = orderDetails[1].ProductID,
                        OrderID = orderDetails[1].OrderID,
                        Rating = 4,
                        Comment = "Chất lượng tốt, giao hàng hơi chậm",
                        CreatedTime = DateTime.UtcNow.AddDays(-9)
                    },
                    new Review
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserID = users[2].Id,
                        OrderDetailID = orderDetails[2].Id,
                        ProductsID = orderDetails[2].ProductID,
                        OrderID = orderDetails[2].OrderID,
                        Rating = 5,
                        Comment = "Sữa rất ngon, con uống rất thích",
                        CreatedTime = DateTime.UtcNow.AddDays(-8)
                    },
                    new Review
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserID = users[0].Id,
                        OrderDetailID = orderDetails[3].Id,
                        ProductsID = orderDetails[3].ProductID,
                        OrderID = orderDetails[3].OrderID,
                        Rating = 3,
                        Comment = "Sản phẩm tạm được, cần cải thiện bao bì",
                        CreatedTime = DateTime.UtcNow.AddDays(-7)
                    },
                    new Review
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserID = users[1].Id,
                        OrderDetailID = orderDetails[4].Id,
                        ProductsID = orderDetails[4].ProductID,
                        OrderID = orderDetails[4].OrderID,
                        Rating = 5,
                        Comment = "Chất lượng tuyệt vời, giá cả hợp lý",
                        CreatedTime = DateTime.UtcNow.AddDays(-6)
                    },
                    new Review
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserID = users[2].Id,
                        OrderDetailID = orderDetails[5].Id,
                        ProductsID = orderDetails[5].ProductID,
                        OrderID = orderDetails[5].OrderID,
                        Rating = 4,
                        Comment = "Sản phẩm tốt, đúng như mô tả",
                        CreatedTime = DateTime.UtcNow.AddDays(-5)
                    },
                    new Review
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserID = users[0].Id,
                        OrderDetailID = orderDetails[6].Id,
                        ProductsID = orderDetails[6].ProductID,
                        OrderID = orderDetails[6].OrderID,
                        Rating = 5,
                        Comment = "Rất hài lòng với chất lượng sản phẩm",
                        CreatedTime = DateTime.UtcNow.AddDays(-4)
                    },
                    new Review
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserID = users[1].Id,
                        OrderDetailID = orderDetails[7].Id,
                        ProductsID = orderDetails[7].ProductID,
                        OrderID = orderDetails[7].OrderID,
                        Rating = 4,
                        Comment = "Sẽ ủng hộ shop lần sau",
                        CreatedTime = DateTime.UtcNow.AddDays(-3)
                    },
                    new Review
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserID = users[2].Id,
                        OrderDetailID = orderDetails[8].Id,
                        ProductsID = orderDetails[8].ProductID,
                        OrderID = orderDetails[8].OrderID,
                        Rating = 5,
                        Comment = "Giao hàng nhanh, nhân viên thân thiện",
                        CreatedTime = DateTime.UtcNow.AddDays(-2)
                    },
                    new Review
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserID = users[0].Id,
                        OrderDetailID = orderDetails[9].Id,
                        ProductsID = orderDetails[9].ProductID,
                        OrderID = orderDetails[9].OrderID,
                        Rating = 4,
                        Comment = "Đóng gói cẩn thận, sản phẩm chất lượng",
                        CreatedTime = DateTime.UtcNow.AddDays(-1)
                    }
                };

                await context.Reviews.AddRangeAsync(reviews);
                await context.SaveChangesAsync();
            }
        }
    }
}