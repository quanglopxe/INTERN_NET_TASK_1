using Microsoft.AspNetCore.Identity;
using MilkStore.ModelViews.AuthModelViews;
using MilkStore.Repositories.Entity;

public interface IAuthService
{
    Task<ApplicationUser> ExistingUser(string userName);
    Task ChangePasswordAdmin(string id, ChangePasswordAdminModel model);
    Task<SignInResult> CheckPassword(LoginModelView loginModel);
    (string token, IEnumerable<string> roles) GenerateJwtToken(ApplicationUser user);
}