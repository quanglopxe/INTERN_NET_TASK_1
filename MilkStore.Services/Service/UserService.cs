using Microsoft.AspNetCore.Identity;
using XuongMay.Contract.Repositories.Entity;
using XuongMay.Contract.Repositories.Interface;
using XuongMay.Contract.Services.Interface;
using XuongMay.ModelViews.UserModelViews;
using XuongMay.Repositories.Context;
using XuongMay.Repositories.Entity;

namespace XuongMay.Services.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly DatabaseContext context;
        public UserService(DatabaseContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IUnitOfWork unitOfWork)
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
            _unitOfWork = unitOfWork;
        }
        public async Task<ApplicationUser> GetUserByEmail(string email)
        {
            return await userManager.FindByEmailAsync(email);
        }
        public async Task<IdentityResult> CreateUser(RegisterModelView userModel)
        {
            var newUser = new ApplicationUser
            {
                UserName = userModel.Username,
                Email = userModel.Email,
                PhoneNumber = userModel.PhoneNumber
            };

            var result = await userManager.CreateAsync(newUser, userModel.Password);
            if (result.Succeeded)
            {
                var roleExist = await roleManager.RoleExistsAsync("Member");
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new ApplicationRole { Name = "Member" });
                }
                await userManager.AddToRoleAsync(newUser, "Member");
            }
            return result;
        }
    }
}
