using System.ComponentModel.DataAnnotations;

namespace VehicleRepairMVC.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? ImagePath { get; set;}
    }
}
