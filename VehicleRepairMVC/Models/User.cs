using System.ComponentModel.DataAnnotations;

namespace VehicleRepairMVC.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı boş bırakılamaz!")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Şifre boş bırakılamaz!")]
        public string Password { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
