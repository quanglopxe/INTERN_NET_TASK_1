using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Core.Utils;
using MilkStore.Repositories.Context;
using MilkStore.Repositories.Entity;

namespace MilkStore.Services.Configs
{
    public static class SeedDataPost
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            DatabaseContext? context = serviceProvider.GetRequiredService<DatabaseContext>();

            if (!context.Posts.Any())
            {
                List<ApplicationUser>? users = await context.ApplicationUsers.Take(2).ToListAsync();
                List<Products>? products = await context.Products.Take(3).ToListAsync();

                List<Post>? posts = new List<Post>
                {
                    new Post
                    {
                        Id = Guid.NewGuid().ToString(),
                        Title = "Khuyến mãi đặc biệt tháng 6",
                        Content = "Giảm giá sốc cho tất cả sản phẩm sữa bột",
                        Image = "promotion-june.jpg",
                        CreatedTime = DateTime.UtcNow,
                        CreatedBy = users[0].Id.ToString()
                    },
                    new Post
                    {
                        Id = Guid.NewGuid().ToString(),
                        Title = "Sản phẩm mới: Sữa hạt organic",
                        Content = "Ra mắt dòng sản phẩm sữa hạt organic cao cấp",
                        Image = "new-organic.jpg",
                        CreatedTime = DateTime.UtcNow,
                        CreatedBy = users[1].Id.ToString()
                    },
                    new Post
                    {
                        Id = Guid.NewGuid().ToString(),
                        Title = "Hướng dẫn bảo quản sữa",
                        Content = "Các tips bảo quản sữa trong mùa hè",
                        Image = "milk-tips.jpg",
                        CreatedTime = DateTime.UtcNow,
                        CreatedBy = users[0].Id.ToString()
                    },
                    new Post
                    {
                        Id = Guid.NewGuid().ToString(),
                        Title = "Chương trình tích điểm",
                        Content = "Tích điểm đổi quà cùng MilkStore",
                        Image = "points.jpg",
                        CreatedTime = DateTime.UtcNow,
                        CreatedBy = users[1].Id.ToString()
                    },
                    new Post
                    {
                        Id = Guid.NewGuid().ToString(),
                        Title = "Sữa và sức khỏe",
                        Content = "Tầm quan trọng của sữa đối với sức khỏe",
                        Image = "health.jpg",
                        CreatedTime = DateTime.UtcNow,
                        CreatedBy = users[0].Id.ToString()
                    },
                    new Post
                    {
                        Id = Guid.NewGuid().ToString(),
                        Title = "Flash Sale cuối tuần",
                        Content = "Giảm giá 50% cho sữa tươi vào cuối tuần",
                        Image = "flash-sale.jpg",
                        CreatedTime = DateTime.UtcNow,
                        CreatedBy = users[1].Id.ToString()
                    },
                    new Post
                    {
                        Id = Guid.NewGuid().ToString(),
                        Title = "Combo tiết kiệm",
                        Content = "Mua 2 tặng 1 cho tất cả sản phẩm sữa chua",
                        Image = "combo.jpg",
                        CreatedTime = DateTime.UtcNow,
                        CreatedBy = users[0].Id.ToString()
                    },
                    new Post
                    {
                        Id = Guid.NewGuid().ToString(),
                        Title = "Chọn sữa cho bé",
                        Content = "Hướng dẫn chọn sữa phù hợp cho trẻ em",
                        Image = "kids-milk.jpg",
                        CreatedTime = DateTime.UtcNow,
                        CreatedBy = users[1].Id.ToString()
                    },
                    new Post
                    {
                        Id = Guid.NewGuid().ToString(),
                        Title = "Ưu đãi sinh nhật",
                        Content = "Tặng voucher 100k cho khách hàng sinh nhật",
                        Image = "birthday.jpg",
                        CreatedTime = DateTime.UtcNow,
                        CreatedBy = users[0].Id.ToString()
                    },
                    new Post
                    {
                        Id = Guid.NewGuid().ToString(),
                        Title = "Sữa nhập khẩu",
                        Content = "Các sản phẩm sữa nhập khẩu chính hãng",
                        Image = "import.jpg",
                        CreatedTime = DateTime.UtcNow,
                        CreatedBy = users[1].Id.ToString()
                    }
                };

                await context.Posts.AddRangeAsync(posts);
                await context.SaveChangesAsync();

            }
        }
    }
}