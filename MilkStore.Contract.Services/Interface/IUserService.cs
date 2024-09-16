using Microsoft.AspNetCore.Identity;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.ModelViews.AuthModelViews;
using MilkStore.ModelViews.ResponseDTO;
using MilkStore.ModelViews.UserModelViews;
using MilkStore.Repositories.Entity;

namespace MilkStore.Contract.Services.Interface
{
    public interface IUserService
    {
        Task<ApplicationUser> GetUserByEmail(string email);
        Task<IdentityResult> CreateUser(RegisterModelView userModel);
        Task<ApplicationUser> UpdateUser(Guid id, UserModelView userModel, string updatedBy);
        Task<UserResponeseDTO> DeleteUser(Guid userId, string deleteby);
        Task<IEnumerable<UserResponeseDTO>> GetUser(string? id);
        Task<UserResponeseDTO> AddUser(UserModelView userModel, string createdBy);


    }
}
