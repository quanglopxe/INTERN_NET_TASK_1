using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MilkStore.Contract.Repositories.Entity;
using MilkStore.Contract.Repositories.Interface;
using MilkStore.Contract.Services.Interface;
using MilkStore.Core.Utils;
using MilkStore.ModelViews.AuthModelViews;
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
        private readonly DatabaseContext context;
        private readonly IUserService _userService;

        public UserService(DatabaseContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IUnitOfWork unitOfWork)
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
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

            var result = await userManager.CreateAsync(newUser, userModel.Password);
            if (result.Succeeded)
            {
                var roleExist = await roleManager.RoleExistsAsync("Member");
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new ApplicationRole { Name = "Member" });
                }
                await userManager.AddToRoleAsync(newUser, "Member");
            }
            return result;
        }
        // Cập nhật thông tin người dùng
        public async Task<User> UpdateUser(string id, UserModelView userModel, string updatedBy)
        {
            var user = await _unitOfWork.GetRepository<User>().GetByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {id} was not found.");
            }

            user.FullName = userModel.FullName;
            user.Email = userModel.Email;
            user.PasswordHash = userModel.PasswordHash;
            user.Address = userModel.Address;
            user.PhoneNumber = userModel.PhoneNumber;
            user.Points = 0;
            user.LastUpdatedTime = CoreHelper.SystemTimeNow;
            user.LastUpdatedBy = updatedBy;

            await _unitOfWork.GetRepository<User>().UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            return user;
        }




        // Xóa người dùng
        public async Task<User> DeleteUser(string userId, string createdBy)
        {
            var user = await _unitOfWork.GetRepository<User>().GetByIdAsync(userId);
            if (user == null)
            {
                return null; 
            }

            user.DeletedTime = DateTimeOffset.UtcNow;
            user.DeletedBy = createdBy;

            await _unitOfWork.GetRepository<User>().UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            return user;
        }

        // Lấy thông tin người dùng theo ID
        public async Task<IEnumerable<User>> GetUser(string? id)
        {
            if (id == null)
            {
                return await _unitOfWork.GetRepository<User>().Entities.Where(u => u.DeletedTime == null).ToListAsync();
            }
            else
            {
                var user = await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(u => u.Id == id && u.DeletedTime == null);
                return user != null ? new List<User> { user } : new List<User>();
            }
        }
        public async Task<User> AddUser(UserModelView userModel, string createdBy)
        {
            var newUser = new User
            {
                FullName = userModel.FullName,
                Email = userModel.Email,
                PasswordHash = userModel.PasswordHash,
                Address = userModel.Address,
                PhoneNumber = userModel.PhoneNumber,
                Points = 0,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy, 
                CreatedTime = DateTimeOffset.UtcNow
            };

            await _unitOfWork.GetRepository<User>().InsertAsync(newUser);
            await _unitOfWork.SaveAsync();

            return newUser;
        }
    }


 }

