using System.ComponentModel.DataAnnotations;

public class ConfirmEmailModel
{
    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Token không được để trống")]
    public string Token { get; set; }
}