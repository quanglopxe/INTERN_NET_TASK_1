using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Repositories.Entity;

namespace MilkStore.Services.Configs
{
    public static class SeedDataAccount
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            UserManager<ApplicationUser> userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            RoleManager<ApplicationRole> roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = "Admin" });
            }
            string email = "Admin@gmail.com";
            string password = "Admin123.";

            ApplicationUser? adminAccount = await userManager.FindByEmailAsync(email);
            if (adminAccount is null)
            {
                ApplicationUser? newAccount = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                };
                await userManager.CreateAsync(newAccount, password);
                await userManager.AddToRoleAsync(newAccount, "Admin");
            }
        }
    }
}