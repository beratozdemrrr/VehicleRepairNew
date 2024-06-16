using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleRepairMVC.Models
{
    public class RepairProcess
    {
        [Key]
        public int RepairProcessID { get; set; }
        public string RepairProcessDescription { get; set; }
        public DateTime RepairProcessTime { get; set; }
        public decimal ProcessPrice { get; set; }

        [ForeignKey("Appointment")]
        public int? AppointmentID { get; set; }
        public Appointment? Appointment { get; set; }
    }
}
