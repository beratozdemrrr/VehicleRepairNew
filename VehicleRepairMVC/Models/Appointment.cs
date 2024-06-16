using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleRepairMVC.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentID { get; set; }

        [ForeignKey("Car")]
        public int? CarID { get; set; }

        public Car? Car { get; set; }

        public DateTime AppointmentDate { get; set; }
        public string Description { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<RepairProcess>? RepairProcesses { get; set; }
    }
}
