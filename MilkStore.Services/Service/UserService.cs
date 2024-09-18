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
using System.ComponentModel.DataAnnotations;

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
        public async Task<ApplicationUser> UpdateUser(Guid id, UserModelView userModel, string updatedBy)
        {
            if (string.IsNullOrWhiteSpace(id.ToString()))
            {
                throw new KeyNotFoundException("User ID cannot be null, empty, or contain only whitespace.");
            }

            var user = await _unitOfWork.GetRepository<ApplicationUser>().GetByIdAsync(id)
              ?? throw new KeyNotFoundException($"User with ID {id} was not found.");



            user.UserName = userModel.UserName;
            user.Email = userModel.Email;
            // user.PasswordHash = userModel.PasswordHash;
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
            if (string.IsNullOrWhiteSpace(userId.ToString()))
            {
                throw new KeyNotFoundException("User ID cannot be null, empty, or contain only whitespace.");
            }
            var user = await _unitOfWork.GetRepository<ApplicationUser>().GetByIdAsync(userId)
                ?? throw new KeyNotFoundException("User does not exist or has already been deleted.");

            if (user.DeletedTime.HasValue)
            {
                throw new KeyNotFoundException("User does not exist or has already been deleted.");
            }


            user.DeletedTime = DateTimeOffset.UtcNow;
            user.DeletedBy = deleteby;

            await _unitOfWork.GetRepository<ApplicationUser>().UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            return user;
        }

        // Lấy thông tin người dùng theo ID
        public async Task<IEnumerable<UserResponeseDTO>> GetUser(string? id, int index = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                IQueryable<ApplicationUser> usersQuery = _unitOfWork.GetRepository<ApplicationUser>().Entities
                    .Where(u => u.DeletedTime == null);

                var paginatedUsers = await _unitOfWork.GetRepository<ApplicationUser>().GetPagging(usersQuery, index, pageSize);

                List<UserResponeseDTO> userResponseDtos = paginatedUsers.Items
                    .Select(MapToUserResponseDto)
                    .ToList();

                return userResponseDtos;
            }
            else
            {
                ApplicationUser user = await _unitOfWork.GetRepository<ApplicationUser>().Entities
                    .FirstOrDefaultAsync(u => u.Id.ToString() == id && u.DeletedTime == null)
                    ?? throw new KeyNotFoundException($"User with ID {id} was not found.");

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
        public async Task<ApplicationUser> AddUser(UserModelView userModel, string createdBy)
        {
            bool emailExists = await _unitOfWork.GetRepository<ApplicationUser>().Entities
                .AnyAsync(u => u.Email == userModel.Email);

            if (emailExists)
            {
                throw new ArgumentException("Email already exists");
            }

            bool userNameExists = await _unitOfWork.GetRepository<ApplicationUser>().Entities
                .AnyAsync(u => u.UserName == userModel.UserName);

            if (userNameExists)
            {
                throw new ArgumentException("UserName already exists");
            }
            ApplicationUser newUser = new ApplicationUser
            {
                UserName = userModel.UserName,
                Email = userModel.Email,
                Password= userModel.Password,
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

        public async Task AccumulatePoints(Guid userId, double totalAmount)
        {
            // Kiểm tra nếu totalAmount <= 0 thì không cần cộng điểm
            if (totalAmount <= 0)
            {
                return;
            }

            ApplicationUser user = await _unitOfWork.GetRepository<ApplicationUser>().GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            // Tính điểm thưởng: 10 điểm cho mỗi 10.000 VND
            int earnedPoints = (int)(totalAmount / 10000) * 10;

            user.Points += earnedPoints;

            await _unitOfWork.GetRepository<ApplicationUser>().UpdateAsync(user);
            await _unitOfWork.SaveAsync();
        }

    }


}
