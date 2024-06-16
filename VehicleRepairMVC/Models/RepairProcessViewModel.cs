namespace VehicleRepairMVC.Models
{
    public class RepairProcessViewModel
    {
        public int? AppointmentID { get; set; }
        public string? RepairProcessDescription { get; set; }
        public DateTime RepairProcessTime { get; set; }
        public decimal ProcessPrice { get; set; }
    }
}
