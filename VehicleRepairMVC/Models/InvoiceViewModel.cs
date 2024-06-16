namespace VehicleRepairMVC.Models
{
    public class InvoiceViewModel
    {
        public string UserName { get; set; }
        public string UserFullName { get; set; }
        public string LicensePlate { get; set; }
        public DateTime AppointmentDate { get; set; }
        public List<RepairProcess> RepairProcesses { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
