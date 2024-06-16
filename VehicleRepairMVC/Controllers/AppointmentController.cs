using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VehicleRepairMVC.Context;
using VehicleRepairMVC.Models;

namespace VehicleRepairMVC.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly VehicleRepairDbContext _context;

        public AppointmentController(VehicleRepairDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.IsLoggedIn = HttpContext.Session.GetString("IsLoggedIn") == "true";
            return View();
        }

        [HttpPost]
        public ActionResult Index(Car car)
        {
            ViewBag.IsLoggedIn = HttpContext.Session.GetString("IsLoggedIn") == "true";
            int? loggedInUserId = HttpContext.Session.GetInt32("LoggedInUserId");

            car.UserID = loggedInUserId;


            if (ModelState.IsValid)
            {
                _context.Cars.Add(car);
                _context.SaveChanges();
                TempData["CarSuccessMessage"] = "Arabanız başarıyla eklendi. Randevu alabilirsiniz.";
                return RedirectToAction("AddAppointment");
            }

            return View(car);

        }

        public IActionResult AddAppointment() 
        {
            ViewBag.IsLoggedIn = HttpContext.Session.GetString("IsLoggedIn") == "true";
            int? loggedInUserId = HttpContext.Session.GetInt32("LoggedInUserId");

            var cars = _context.Cars
         .Where(c => c.UserID == loggedInUserId.Value)
         .Select(c => new SelectListItem
         {
             Value = c.CarID.ToString(),
             Text = c.LicensePlate
         })
         .ToList();

            ViewBag.Cars = cars;

            return View();
        }

        [HttpPost]
        public IActionResult AddAppointment(Appointment appointment)
        {
            ViewBag.IsLoggedIn = HttpContext.Session.GetString("IsLoggedIn") == "true";
            int? loggedInUserId = HttpContext.Session.GetInt32("LoggedInUserId");

            appointment.CreatedAt= DateTime.Now;
            appointment.Status = "Onay bekliyor";
            if (ModelState.IsValid)
            {
                _context.Appointments.Add(appointment);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Randevu isteğiniz başarıyla gönderildi. Admin işlem yaptıktan sonra randevularım veya bildirim kısmından randevu durumunu öğrenebilirsiniz.";
                return RedirectToAction("AddAppointment");
            }
            
            var cars = _context.Cars
           .Where(c => c.UserID == loggedInUserId.Value)
           .Select(c => new SelectListItem
           {
               Value = c.CarID.ToString(),
               Text = c.LicensePlate
           })
           .ToList();

            ViewBag.Cars = cars;

            return View(appointment);
        }
    }
}
