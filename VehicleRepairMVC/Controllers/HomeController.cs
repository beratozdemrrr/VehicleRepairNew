using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VehicleRepairMVC.Context;
using VehicleRepairMVC.Models;

namespace VehicleRepairMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly VehicleRepairDbContext _context;

        public HomeController(ILogger<HomeController> logger, VehicleRepairDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var isLoggedIn = HttpContext.Session.GetString("IsLoggedIn");
            if (isLoggedIn == "true")
            {
                ViewBag.IsLoggedIn = true;
                ViewBag.Username = HttpContext.Session.GetString("Username");
            }
            else
            {
                ViewBag.IsLoggedIn = false;
            }

            return View();
        }

        public IActionResult Products()
        {
            ViewBag.IsLoggedIn = HttpContext.Session.GetString("IsLoggedIn") == "true";
            var products=_context.Products.ToList();
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}