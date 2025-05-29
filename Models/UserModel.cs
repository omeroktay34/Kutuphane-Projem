using System.ComponentModel.DataAnnotations;

namespace kutuphane.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre boş olamaz.")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string Password { get; set; }


        public string? FullName { get; set; }
        public string? Bio { get; set; }
        public byte[]? ProfileImage { get; set; }


        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }




public string FontFamily { get; set; } = "Poppins";

public int FontSize { get; set; } = 16;         


public string? Theme { get; set; }  
public string? Language { get; set; }  




    }



}