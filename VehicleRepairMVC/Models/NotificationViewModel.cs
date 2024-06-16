namespace VehicleRepairMVC.Models
{
    public class NotificationViewModel
    {
        public int NotificationID { get; set; }
        public string NotificationText { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string LicensePlate { get; set; }
    }
}
