using System.ComponentModel.DataAnnotations;

namespace MilkStore.ModelViews.UserModelViews;
public class UserUpdateModelView
{
    [Required(ErrorMessage = "PhoneNumber is required")]
    [Phone(ErrorMessage = "Phone number is invalid")]
    public string? PhoneNumber { get; set; }
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; }

    [EmailAddress(ErrorMessage = "Email is not valid")]
    public string Email { get; set; }

    [Required(ErrorMessage = "ShippingAddress is required")]
    public string ShippingAddress { get; set; }
}