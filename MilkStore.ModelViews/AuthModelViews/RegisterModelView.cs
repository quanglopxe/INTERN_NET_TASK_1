using System.ComponentModel.DataAnnotations;

public class RegisterModelView
{
    [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
    public string Username { get; set; }
    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    public string Password { get; set; }
    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Số điện thoại không được để trống")]
    [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
    public string PhoneNumber { get; set; }
}