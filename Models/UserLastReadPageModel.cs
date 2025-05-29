using System.ComponentModel.DataAnnotations;

namespace kutuphane.Models
{
    public class UserLastReadPageModel
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        
        public string? FullName { get; set; }
        public string? Bio { get; set; }
        public byte[]? ProfileImage { get; set; }

    }



}