using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleRepairMVC.Models
{
    public class Car
    {
        [Key]
        public int CarID { get; set; }

        [ForeignKey("User")]
        public int? UserID { get; set; }

        public User? User { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public int Year { get; set; }
        public string? LicensePlate { get; set; }

    }
}
