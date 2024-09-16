using Microsoft.AspNetCore.Identity;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.ModelViews.AuthModelViews;
using MilkStore.ModelViews.UserModelViews;
using MilkStore.Repositories.Entity;

namespace MilkStore.Contract.Services.Interface
{
    public interface IUserService
    {
        Task<ApplicationUser> GetUserByEmail(string email);
        Task<IdentityResult> CreateUser(RegisterModelView userModel);
        Task<ApplicationUser> UpdateUser(Guid id, UserModelView userModel, string updatedBy);
        Task<ApplicationUser> DeleteUser(Guid userId, string deleteby);
        Task<IEnumerable<ApplicationUser>> GetUser(string? id);
        Task<ApplicationUser> AddUser(UserModelView userModel, string createdBy);


    }
}
