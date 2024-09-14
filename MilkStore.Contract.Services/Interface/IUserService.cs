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
        Task<User> UpdateUser(string id, UserModelView userModel, string updatedBy);
        Task<User> DeleteUser(string userId, string deleteby);
        Task<IEnumerable<User>> GetUser(Guid? id);
        Task<User> AddUser(UserModelView userModel, string createdBy);


    }
}
