﻿using AutoMapper;
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
                var roleExist = await roleManager.RoleExistsAsync("Staff");
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new ApplicationRole { Name = "Staff" });
                }
                await userManager.AddToRoleAsync(newUser, "Staff");
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
        public async Task<ApplicationUser> DeleteUser(Guid userId, string deleteby)
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

            return user;
        }

        // Lấy thông tin người dùng theo ID
        public async Task<IEnumerable<ApplicationUser>> GetUser(string? id)
        {
            if (id == null)
            {
                return await _unitOfWork.GetRepository<ApplicationUser>().Entities.Where(u => u.DeletedTime == null).ToListAsync();
            }
            else
            {
                var user = await _unitOfWork.GetRepository<ApplicationUser>().Entities.FirstOrDefaultAsync(u => u.Id.ToString() == id && u.DeletedTime == null);
                return user != null ? new List<ApplicationUser> { user } : new List<ApplicationUser>();
            }
        }
        public async Task<ApplicationUser> AddUser(UserModelView userModel, string createdBy)
        {
            var newUser = new ApplicationUser
            {
                UserName = userModel.UserName,
                Email = userModel.Email,
                PasswordHash = userModel.PasswordHash,
                PhoneNumber = userModel.PhoneNumber,    
                Points = 0,
                CreatedBy = createdBy, 
                CreatedTime = DateTimeOffset.UtcNow
            };

            await _unitOfWork.GetRepository<ApplicationUser>().InsertAsync(newUser);
            await _unitOfWork.SaveAsync();

            return newUser;
        }
    }


 }

