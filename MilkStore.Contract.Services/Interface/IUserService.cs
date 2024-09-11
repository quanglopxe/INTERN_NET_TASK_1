using Microsoft.AspNetCore.Identity;
using MilkStore.ModelViews.AuthModelViews;
using MilkStore.ModelViews.UserModelViews;
using MilkStore.Repositories.Entity;

namespace MilkStore.Contract.Services.Interface
{
    public interface IUserService
    {
        Task<ApplicationUser> GetUserByEmail(string email);
        Task<IdentityResult> CreateUser(RegisterModelView userModel);
    }
}
