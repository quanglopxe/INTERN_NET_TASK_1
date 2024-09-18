using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Repositories.Interface;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core.Base;
using MilkStore.Core.Utils;
using MilkStore.ModelViews.AuthModelViews;
using MilkStore.ModelViews.ResponseDTO;
using MilkStore.ModelViews.UserModelViews;
using MilkStore.Repositories.Context;
using MilkStore.Repositories.Entity;

namespace MilkStore.Services.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        public UserService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IUnitOfWork unitOfWork, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            _unitOfWork = unitOfWork;
        }
        public async Task GetUserByEmailToRegister(string email)
        {
            ApplicationUser? user = await userManager.FindByNameAsync(email);
            if (user != null)
            {
                throw new BaseException.ErrorException(400, "BadRequest", "Email đã tồn tại!");
            }
        }
        public async Task CreateUser(RegisterModelView userModel)
        {
            ApplicationUser? newUser = new ApplicationUser
            {
                UserName = userModel.Email,
                Email = userModel.Email,
                PhoneNumber = userModel.PhoneNumber
            };

            IdentityResult? result = await userManager.CreateAsync(newUser, userModel.Password);
            if (result.Succeeded)
            {
                bool roleExist = await roleManager.RoleExistsAsync("Member");
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new ApplicationRole { Name = "Member" });
                }
                await userManager.AddToRoleAsync(newUser, "Member");
            }
            else
            {
                throw new BaseException.ErrorException(500, "InternalServerError", $"Lỗi khi tạo người dùng {result.Errors.FirstOrDefault()?.Description}");
            }
        }
        public async Task<IdentityResult> CreateUserLoginGoogle(LoginGoogleModel loginGoogleModel)
        {
            ApplicationUser? newUser = new ApplicationUser
            {
                UserName = loginGoogleModel.Gmail,
                Email = loginGoogleModel.Gmail,
            };
            if (string.IsNullOrEmpty(loginGoogleModel.ProviderKey))
            {
                throw new Exception("Provider key not found in claims");
            }
            IdentityResult? result = await userManager.CreateAsync(newUser);
            if (result.Succeeded)
            {
                string roleName = "Member";
                bool roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new ApplicationRole { Name = roleName });
                }
                await userManager.AddToRoleAsync(newUser, roleName);
                UserLoginInfo? userInfoLogin = new("Google", loginGoogleModel.ProviderKey, "Google");
                IdentityResult loginResult = await userManager.AddLoginAsync(newUser, userInfoLogin);
                if (!loginResult.Succeeded)
                {
                    return loginResult;
                }
            }
            return result;
        }
        // Cập nhật thông tin người dùng
        public async Task<ApplicationUser> UpdateUser(Guid id, UserModelView userModel, string updatedBy)
        {
            var user = await _unitOfWork.GetRepository<ApplicationUser>().GetByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {id} was not found.");
            }

            user.UserName = userModel.UserName;
            user.Email = userModel.Email;
            //  user.PasswordHash = userModel.PasswordHash;
            user.PhoneNumber = userModel.PhoneNumber;
            user.Points = 0;
            user.LastUpdatedTime = CoreHelper.SystemTimeNow;
            user.LastUpdatedBy = updatedBy;

            await _unitOfWork.GetRepository<ApplicationUser>().UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            return user;
        }




        // Xóa người dùng
        public async Task<UserResponeseDTO> DeleteUser(Guid userId, string deleteby)
        {
            var user = await _unitOfWork.GetRepository<ApplicationUser>().GetByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            user.DeletedTime = DateTimeOffset.UtcNow;
            user.DeletedBy = deleteby;

            await _unitOfWork.GetRepository<ApplicationUser>().UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            return MapToUserResponseDto(user);
        }

        // Lấy thông tin người dùng theo ID
        public async Task<IEnumerable<UserResponeseDTO>> GetUser(string? id)
        {
            if (id == null)
            {
                var user = await _unitOfWork.GetRepository<ApplicationUser>().Entities
                    .Where(u => u.DeletedTime == null)
                    .ToListAsync();

                // Ánh xạ listPosts sang kiểu trả về khác
                return user.Select(MapToUserResponseDto).ToList();
            }
            else
            {
                var user = await _unitOfWork.GetRepository<ApplicationUser>().Entities.FirstOrDefaultAsync(u => u.Id.ToString() == id && u.DeletedTime == null);
                if (user == null)
                {
                    throw new KeyNotFoundException($"Post with ID {id} was not found.");
                }
                return new List<UserResponeseDTO> { MapToUserResponseDto(user) };
            }
        }

        private UserResponeseDTO MapToUserResponseDto(ApplicationUser user)
        {
            return new UserResponeseDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Points = user.Points,
                CreatedBy = user.CreatedBy,
                DeletedBy = user.DeletedBy,
                LastUpdatedTime = user.LastUpdatedTime,
                CreatedTime = user.CreatedTime,

            };
        }
        public async Task<UserResponeseDTO> AddUser(UserModelView userModel, string createdBy)
        {
            var newUser = new ApplicationUser
            {
                UserName = userModel.UserName,
                Email = userModel.Email,
                Password = userModel.Password,
                PasswordHash = userModel.PasswordHash,
                PhoneNumber = userModel.PhoneNumber,
                Points = 0,
                CreatedBy = createdBy,
                CreatedTime = DateTimeOffset.UtcNow
            };

            await _unitOfWork.GetRepository<ApplicationUser>().InsertAsync(newUser);
            await _unitOfWork.SaveAsync();

            return MapToUserResponseDto(newUser);
        }
    }


}
