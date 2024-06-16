using Microsoft.AspNetCore.Mvc;
using VehicleRepairMVC.Context;
using VehicleRepairMVC.Models;

namespace VehicleRepairMVC.Controllers
{
    public class NotificationController : Controller
    {
        private readonly VehicleRepairDbContext _context;

        public NotificationController(VehicleRepairDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.IsLoggedIn = HttpContext.Session.GetString("IsLoggedIn") == "true";

            if (HttpContext.Session.GetInt32("LoggedInUserId").HasValue)
            {
                int loggedInUserId = HttpContext.Session.GetInt32("LoggedInUserId").Value;

                var userNotifications = _context.Notifications
                    .Where(n => n.UserID == loggedInUserId)
                    .Select(n => new NotificationViewModel
                    {
                        NotificationID = n.NotificationID,
                        NotificationText = n.NotificationText,
                        CreatedAt = n.CreatedAt,
                        AppointmentDate = n.Appointment.AppointmentDate,
                        LicensePlate = n.Appointment.Car.LicensePlate
                    })
                    .ToList();

                return View(userNotifications);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

        }
    }
}
