using System.ComponentModel.DataAnnotations;

namespace MilkStore.ModelViews.UserModelViews
{
    public class UserUpdateByAdminModel : UserUpdateModelView
    {
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&.])[A-Za-z\d@$!%*?&.]{8,16}$", ErrorMessage = "Password must be at least 8 characters, 1 uppercase, 1 lowercase, 1 number and 1 special character")]
        public string? Password { get; set; }
    }
}