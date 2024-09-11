using Microsoft.AspNetCore.Identity;
using XuongMay.ModelViews.AuthModelViews;
using XuongMay.Repositories.Entity;

public interface IAuthService
{
    Task<ApplicationUser> CheckUser(string userName);
    Task<SignInResult> CheckPassword(LoginModelView loginModel);
    (string token, IList<string> roles) GenerateJwtToken(ApplicationUser user);
}