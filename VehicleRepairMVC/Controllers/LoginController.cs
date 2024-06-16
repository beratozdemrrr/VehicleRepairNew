using Microsoft.AspNetCore.Mvc;
using VehicleRepairMVC.Context;
using VehicleRepairMVC.Models;

namespace VehicleRepairMVC.Controllers
{
    public class LoginController : Controller
    {
        private readonly VehicleRepairDbContext _context;

        public LoginController(VehicleRepairDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(User user)
        {

            if (ModelState.IsValid)
            { 
                var person = _context.Users.SingleOrDefault(u => u.Username == user.Username && u.Password == user.Password);
                if (person != null) {
                    var LoggedInUserId = person.UserID;
                    HttpContext.Session.SetInt32("LoggedInUserId", LoggedInUserId);
                    HttpContext.Session.SetString("Username", person.Username);
                    HttpContext.Session.SetString("IsLoggedIn", "true");
                    return RedirectToAction("Index","Home");
                }
            else
            {
                    TempData["ErrorMessage"] = "Kullanıcı adı veya şifre yanlış"; //Giriş yapmış kullanıcı yoksa
                    return RedirectToAction("Index","Login");
                }
            }
         
            return View(user);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            
            _context.Users.Add(user);
            _context.SaveChanges();
                TempData["SuccessMessage"] = "Başarıyla kayıt oldunuz. Giriş yapabilirsiniz.";
            return RedirectToAction("Register","Login");

        }

        public IActionResult AdminLogin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AdminLogin(AdminLoginModel model)
        {
            if (ModelState.IsValid)
            {

                if (IsValidUser(model.Username, model.Password))
                {
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                else
                {
                    TempData["ErrorMessage"] = "Kullanıcı adı veya şifre yanlış";
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }

        private bool IsValidUser(string username, string password)
        {
            // Kullanıcı adı ve şifreyi kontrol et, örneğin bir veritabanından kontrol edilebilir
            // Bu örnekte basit bir kontrol yapısı kullanılmıştır, gerçek projelerde daha güvenli bir yöntem kullanılmalıdır.
            return (username == "admin" && password == "admin123");
        }

        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index","Home");
        }
    }
}
