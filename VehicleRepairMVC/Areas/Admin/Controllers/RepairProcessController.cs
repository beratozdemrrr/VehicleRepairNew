using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleRepairMVC.Context;
using VehicleRepairMVC.Models;

namespace VehicleRepairMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RepairProcessController : Controller
    {
        private readonly VehicleRepairDbContext _context;

        public RepairProcessController(VehicleRepairDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            var appointment = await _context.Appointments
           .Include(a => a.Car)
           .ThenInclude(c => c.User)
           .Where(a => a.Status == "Onaylandı")
           .ToListAsync();

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // Tamir işlemi ekleme formunu gösteren metot
        public async Task<IActionResult> AddRepairProcess(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Car)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(a => a.AppointmentID == id);

            if (appointment == null)
            {
                return NotFound();
            }

            var model = new RepairProcessViewModel
            {
                AppointmentID = appointment.AppointmentID
            };

            return View(model);
        }

        // Tamir işlemi ekleme işlemini gerçekleştiren metot
        [HttpPost]
        public async Task<IActionResult> AddRepairProcess(RepairProcessViewModel model)
        {
            if (ModelState.IsValid)
            {
                var repairProcess = new RepairProcess
                {
                    RepairProcessDescription = model.RepairProcessDescription,
                    RepairProcessTime = model.RepairProcessTime,
                    ProcessPrice = model.ProcessPrice,
                    AppointmentID = model.AppointmentID // Set AppointmentID
                };

                _context.RepairProcesses.Add(repairProcess);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "İşlem başarıyla eklendi.";

                var appointment = await _context.Appointments
                    .Include(a => a.RepairProcesses)
                    .FirstOrDefaultAsync(a => a.AppointmentID == model.AppointmentID);

                if (appointment != null)
                {
                    appointment.RepairProcesses.Add(repairProcess);
                    await _context.SaveChangesAsync();
                }

                var car = await _context.Cars.FindAsync(appointment.CarID);
                if (car != null && car.UserID != null)
                {
                    var notification = new Notification
                    {
                        UserID = (int)car.UserID,
                        AppointmentID = appointment.AppointmentID,
                        NotificationText = repairProcess.RepairProcessDescription,
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    };

                    _context.Notifications.Add(notification);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("AddRepairProcess", "RepairProcess");
            }

            TempData["ErrorMessage"] = "İşlem eklenirken bir hata oluştu.";
            return View(model);
        }
    }
}
