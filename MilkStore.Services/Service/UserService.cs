using Microsoft.AspNetCore.Identity;
using MilkStore.Contract.Repositories.Interface;
using MilkStore.Contract.Services.Interface;
using MilkStore.ModelViews.UserModelViews;
using MilkStore.Repositories.Context;
using MilkStore.Repositories.Entity;

namespace MilkStore.Services.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly DatabaseContext context;
        public UserService(DatabaseContext context, UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            this.context = context;
            this.userManager = userManager;
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

            return await userManager.CreateAsync(newUser, userModel.Password);
        }
    }
}
