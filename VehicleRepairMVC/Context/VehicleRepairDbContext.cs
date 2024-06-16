using Microsoft.EntityFrameworkCore;
using VehicleRepairMVC.Models;

namespace VehicleRepairMVC.Context
{
    public class VehicleRepairDbContext : DbContext
    {
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<RepairProcess> RepairProcesses { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }

        public VehicleRepairDbContext(DbContextOptions<VehicleRepairDbContext> options) : base(options)
        {

        }

    }

   

}
