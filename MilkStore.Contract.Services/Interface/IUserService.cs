using Microsoft.AspNetCore.Identity;
using XuongMay.ModelViews.UserModelViews;
using XuongMay.Repositories.Entity;

namespace XuongMay.Contract.Services.Interface
{
    public interface IUserService
    {
        Task<ApplicationUser> GetUserByEmail(string email);
        Task<IdentityResult> CreateUser(RegisterModelView userModel);
    }
}
