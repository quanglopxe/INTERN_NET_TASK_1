using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Core.Utils;
using MilkStore.Repositories.Context;
using MilkStore.Repositories.Entity;

namespace MilkStore.Services.Configs
{
    public static class SeedDataOrder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            DatabaseContext? context = serviceProvider.GetRequiredService<DatabaseContext>();

            if (!context.Orders.Any())
            {
                List<ApplicationUser>? users = await context.ApplicationUsers.Take(3).ToListAsync();
                List<Products>? products = await context.Products.Take(5).ToListAsync();
                List<Voucher>? vouchers = await context.Vouchers.Take(3).ToListAsync();

                List<Order>? orders = new List<Order>
                {
                    new Order
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = users[0].Id,
                        OrderDate = DateTime.UtcNow.AddDays(-5),
                        ShippingAddress = "123 Đường ABC, Quận 1, TP.HCM",
                        TotalAmount = 250000,
                        DiscountedAmount = 225000,
                        PaymentMethod = PaymentMethod.COD,
                        PaymentStatuss = PaymentStatus.Unpaid,
                        OrderStatuss = OrderStatus.Pending,
                        CreatedTime = CoreHelper.SystemTimeNow.AddDays(-5),
                        estimatedDeliveryDate = CoreHelper.SystemTimeNow.DateTime.AddDays(2).ToString()
                    },
                    new Order
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = users[1].Id,
                        OrderDate = DateTime.UtcNow.AddDays(-4),
                        ShippingAddress = "456 Đường XYZ, Quận 2, TP.HCM",
                        TotalAmount = 500000,
                        DiscountedAmount = 450000,
                        PaymentMethod = PaymentMethod.Online,
                        PaymentStatuss = PaymentStatus.Paid,
                        OrderStatuss = OrderStatus.Confirmed,
                        CreatedTime = CoreHelper.SystemTimeNow.AddDays(-4),
                        estimatedDeliveryDate = CoreHelper.SystemTimeNow.DateTime.AddDays(3).ToString()
                    },
                    new Order
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = users[2].Id,
                        OrderDate = DateTime.UtcNow.AddDays(-3),
                        ShippingAddress = "789 Đường DEF, Quận 3, TP.HCM",
                        TotalAmount = 750000,
                        DiscountedAmount = 675000,
                        PaymentMethod = PaymentMethod.UserWallet,
                        PaymentStatuss = PaymentStatus.Paid,
                        OrderStatuss = OrderStatus.Pending,
                        CreatedTime = CoreHelper.SystemTimeNow.AddDays(-3),
                        estimatedDeliveryDate = CoreHelper.SystemTimeNow.DateTime.AddDays(1).ToString()
                    },
                    new Order
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = users[0].Id,
                        OrderDate = DateTime.UtcNow.AddDays(-2),
                        ShippingAddress = "321 Đường GHI, Quận 4, TP.HCM",
                        TotalAmount = 300000,
                        DiscountedAmount = 270000,
                        PaymentMethod = PaymentMethod.COD,
                        PaymentStatuss = PaymentStatus.Paid,
                        OrderStatuss = OrderStatus.Delivered,
                        CreatedTime = CoreHelper.SystemTimeNow.AddDays(-2),
                        estimatedDeliveryDate = CoreHelper.SystemTimeNow.DateTime.ToString()
                    },
                    new Order
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = users[1].Id,
                        OrderDate = DateTime.UtcNow.AddDays(-1),
                        ShippingAddress = "654 Đường JKL, Quận 5, TP.HCM",
                        TotalAmount = 400000,
                        DiscountedAmount = 360000,
                        PaymentMethod = PaymentMethod.Online,
                        PaymentStatuss = PaymentStatus.Paid,
                        OrderStatuss = OrderStatus.Cancelled,
                        CreatedTime = CoreHelper.SystemTimeNow.AddDays(-1),
                        estimatedDeliveryDate = CoreHelper.SystemTimeNow.DateTime.AddDays(4).ToString()
                    },
                    new Order
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = users[2].Id,
                        OrderDate = DateTime.UtcNow.AddHours(-12),
                        ShippingAddress = "987 Đường MNO, Quận 6, TP.HCM",
                        TotalAmount = 600000,
                        DiscountedAmount = 540000,
                        PaymentMethod = PaymentMethod.UserWallet,
                        PaymentStatuss = PaymentStatus.Paid,
                        OrderStatuss = OrderStatus.Confirmed,
                        CreatedTime = CoreHelper.SystemTimeNow.AddHours(-12),
                        estimatedDeliveryDate = CoreHelper.SystemTimeNow.DateTime.AddDays(3).ToString()
                    },
                    new Order
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = users[0].Id,
                        OrderDate = DateTime.UtcNow.AddHours(-6),
                        ShippingAddress = "147 Đường PQR, Quận 7, TP.HCM",
                        TotalAmount = 800000,
                        DiscountedAmount = 720000,
                        PaymentMethod = PaymentMethod.COD,
                        PaymentStatuss = PaymentStatus.Unpaid,
                        OrderStatuss = OrderStatus.Pending,
                        CreatedTime = CoreHelper.SystemTimeNow.AddHours(-6),
                        estimatedDeliveryDate = CoreHelper.SystemTimeNow.DateTime.AddDays(4).ToString()
                    },
                    new Order
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = users[1].Id,
                        OrderDate = DateTime.UtcNow.AddHours(-3),
                        ShippingAddress = "258 Đường STU, Quận 8, TP.HCM",
                        TotalAmount = 350000,
                        DiscountedAmount = 315000,
                        PaymentMethod = PaymentMethod.Online,
                        PaymentStatuss = PaymentStatus.Paid,
                        OrderStatuss = OrderStatus.Pending,
                        CreatedTime = CoreHelper.SystemTimeNow.AddHours(-3),
                        estimatedDeliveryDate = CoreHelper.SystemTimeNow.DateTime.AddDays(2).ToString()
                    },
                    new Order
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = users[2].Id,
                        OrderDate = DateTime.UtcNow.AddHours(-1),
                        ShippingAddress = "369 Đường VWX, Quận 9, TP.HCM",
                        TotalAmount = 900000,
                        DiscountedAmount = 810000,
                        PaymentMethod = PaymentMethod.UserWallet,
                        PaymentStatuss = PaymentStatus.Paid,
                        OrderStatuss = OrderStatus.Confirmed,
                        CreatedTime = CoreHelper.SystemTimeNow.AddHours(-1),
                        estimatedDeliveryDate = CoreHelper.SystemTimeNow.DateTime.AddDays(3).ToString()
                    },
                    new Order
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = users[0].Id,
                        OrderDate = DateTime.UtcNow,
                        ShippingAddress = "159 Đường YZ, Quận 10, TP.HCM",
                        TotalAmount = 450000,
                        DiscountedAmount = 405000,
                        PaymentMethod = PaymentMethod.COD,
                        PaymentStatuss = PaymentStatus.Unpaid,
                        OrderStatuss = OrderStatus.Pending,
                        CreatedTime = CoreHelper.SystemTimeNow,
                        estimatedDeliveryDate = CoreHelper.SystemTimeNow.DateTime.AddDays(4).ToString()
                    }
                };

                await context.Orders.AddRangeAsync(orders);
                await context.SaveChangesAsync();

                // Seed OrderDetails
                List<OrderDetails>? orderDetails = new List<OrderDetails>();
                foreach (Order order in orders)
                {
                    // Mỗi đơn hàng có 2-3 sản phẩm
                    int numberOfProducts = new Random().Next(2, 4);
                    IEnumerable<Products>? randomProducts = products.OrderBy(x => Guid.NewGuid()).Take(numberOfProducts);

                    foreach (Products product in randomProducts)
                    {
                        var quantity = new Random().Next(1, 5);
                        orderDetails.Add(new OrderDetails
                        {
                            Id = Guid.NewGuid().ToString(),
                            OrderID = order.Id,
                            ProductID = product.Id,
                            Quantity = quantity,
                            UnitPrice = product.Price,
                            Status = order.OrderStatuss == OrderStatus.Cancelled ?
                                    OrderDetailStatus.Cancelled :
                                    OrderDetailStatus.Ordered,
                            CreatedTime = order.CreatedTime,
                            CreatedBy = order.UserId.ToString()
                        });
                    }
                }

                await context.OrderDetails.AddRangeAsync(orderDetails);
                await context.SaveChangesAsync();


            }
        }
    }
}