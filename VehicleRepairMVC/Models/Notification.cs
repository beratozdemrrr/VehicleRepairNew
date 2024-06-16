using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleRepairMVC.Models
{
    public class Notification
    {
        [Key]
        public int NotificationID { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }

        [ForeignKey("Appointment")]
        public int AppointmentID { get; set; }
        public Appointment? Appointment { get; set; }
        public User? User { get; set; }
        public string? NotificationText { get; set; }
        public bool? IsRead { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
