using Microsoft.AspNetCore.Identity;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.ModelViews.AuthModelViews;
using MilkStore.ModelViews.ResponseDTO;
using MilkStore.ModelViews.UserModelViews;
using MilkStore.Repositories.Entity;
using System.Threading.Tasks;

namespace MilkStore.Contract.Services.Interface
{
    public interface IUserService
    {
        Task<ApplicationUser> GetUserByEmail(string email);
        Task<IdentityResult> CreateUser(RegisterModelView userModel);
        Task<ApplicationUser> UpdateUser(Guid id, UserModelView userModel, string updatedBy);
        Task<ApplicationUser> DeleteUser(Guid userId, string deleteby);
        Task<IEnumerable<UserResponeseDTO>> GetUser(string? id, int index = 1, int pageSize = 10);
        Task<ApplicationUser> AddUser(UserModelView userModel, string createdBy);
        Task AccumulatePoints(Guid userId, double totalAmount);

    }
}
