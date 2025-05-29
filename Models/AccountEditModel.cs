using System.ComponentModel.DataAnnotations;

namespace kutuphane.Models
{
    public class AccountEditModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Yeni şifre ve tekrar aynı olmalıdır.")]
        public string? ConfirmNewPassword { get; set; }
    }
}
