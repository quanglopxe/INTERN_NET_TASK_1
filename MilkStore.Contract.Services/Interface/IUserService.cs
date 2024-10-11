using Microsoft.AspNetCore.Identity;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Core;
using MilkStore.Core.Constants;
using MilkStore.ModelViews.AuthModelViews;
using MilkStore.ModelViews.ResponseDTO;
using MilkStore.ModelViews.UserModelViews;
using MilkStore.Repositories.Entity;
using System.Threading.Tasks;

namespace MilkStore.Contract.Services.Interface
{
    public interface IUserService
    {

        Task UpdateUser(UserUpdateModelView userUpdateModelView);
        Task DeleteUser(string userId);
        Task<IEnumerable<UserResponeseDTO>> GetUser(string? id, int index = 1, int pageSize = 10);
        Task AddUserWithRoleAsync(UserModelView userModel);
        Task AccumulatePoints(string userId, double totalAmount);
        Task<UserProfileResponseModelView> GetUserProfile();
        Task<BasePaginatedList<UserResponeseDTO>> GetUserByRole(string roleId, int index, int pageSize);
        Task UpdateUserByAdmin(string userID, UserUpdateByAdminModel model);
        Task<BasePaginatedList<UserResponeseDTO>> SearchUser(string keySearch, SearchUserCode search, int index, int pageSize);

    }
}
