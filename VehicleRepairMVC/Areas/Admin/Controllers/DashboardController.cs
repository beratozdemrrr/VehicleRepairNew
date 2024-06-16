using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using VehicleRepairMVC.Context;

namespace VehicleRepairMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly VehicleRepairDbContext _context;
        public DashboardController(VehicleRepairDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var carUsers = from car in _context.Cars
                           join user in _context.Users on car.UserID equals user.UserID
                           select new
                           {
                               car.Brand,
                               car.Model,
                               car.Year,
                               car.LicensePlate,
                               user.Firstname,
                               user.Lastname,
                               user.Username
                           };

            var expandoList = new List<ExpandoObject>();
            foreach (var item in carUsers)
            {
                dynamic expando = new ExpandoObject();
                expando.Brand = item.Brand;
                expando.Model = item.Model;
                expando.Year = item.Year;
                expando.LicensePlate = item.LicensePlate;
                expando.Firstname = item.Firstname;
                expando.Lastname = item.Lastname;
                expando.Username = item.Username;
                expandoList.Add(expando);
            }

            return View(expandoList);
        }

    }
}
