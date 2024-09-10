using Microsoft.AspNetCore.Identity;
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
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private readonly UserManager<ApplicationUser> userManager;
        private readonly DatabaseContext context;
        public UserService(DatabaseContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
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
