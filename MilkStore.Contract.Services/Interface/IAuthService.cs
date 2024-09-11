using Microsoft.AspNetCore.Identity;
using MilkStore.ModelViews.AuthModelViews;
using MilkStore.Repositories.Entity;

public interface IAuthService
{
    Task<ApplicationUser> CheckUser(string userName);
    Task<SignInResult> CheckPassword(LoginModelView loginModel);
    (string token, IList<string> roles) GenerateJwtToken(ApplicationUser user);
}