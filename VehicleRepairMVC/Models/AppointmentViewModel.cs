namespace VehicleRepairMVC.Models
{
    public class AppointmentViewModel
    {
        public int AppointmentID { get; set; }
        public int CarID { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Username { get; set; }
    }
}
