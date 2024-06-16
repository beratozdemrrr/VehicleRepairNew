using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleRepairMVC.Context;
using VehicleRepairMVC.Models;

namespace VehicleRepairMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AppointmentController : Controller
    {
        private readonly VehicleRepairDbContext _context;

        public AppointmentController(VehicleRepairDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var appointments = _context.Appointments
                .Where(a => a.Status == "Onay Bekliyor") // Sadece "Onay Bekliyor" durumundaki randevuları al
                .Include(a => a.Car)
                .Include(a => a.Car.User)
                .Select(a => new AppointmentViewModel
                {
                    AppointmentID = a.AppointmentID,
                    AppointmentDate = a.AppointmentDate,
                    Description = a.Description,
                    Status = a.Status,
                    CreatedAt = a.CreatedAt,
                    Username = a.Car != null && a.Car.User != null ? a.Car.User.Username : ""
                })
                .ToList();

            return View(appointments);
        }

        public async Task<IActionResult> Approve(int appointmentId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
            {
                return NotFound();
            }

            appointment.Status = "Onaylandı"; // Statusu güncelle
            await _context.SaveChangesAsync();

            var car = await _context.Cars.FindAsync(appointment.CarID);
            if (car != null && car.UserID != null)
            {
                var notification = new Notification
                {
                    UserID = (int)car.UserID,
                    AppointmentID = appointmentId,
                    NotificationText = " başarıyla onaylanmıştır.",
                    CreatedAt = DateTime.Now,
                    IsRead = false
                };
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Reject(int appointmentId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
            {
                return NotFound();
            }

            appointment.Status = "Reddedildi"; // Statusu güncelle
            await _context.SaveChangesAsync();

            // CarID'den Car nesnesini bul ve UserID'yi al
            var car = await _context.Cars.FindAsync(appointment.CarID);
            if (car != null && car.UserID != null)
            {
                var notification = new Notification
                {
                    UserID = (int)car.UserID,
                    AppointmentID = appointmentId,
                    NotificationText = " oluşturulamadı.",
                    CreatedAt = DateTime.Now,
                    IsRead = false
                };
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));

        }
    }
}
