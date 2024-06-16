using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleRepairMVC.Context;
using VehicleRepairMVC.Models;

namespace VehicleRepairMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly VehicleRepairDbContext _context;

        public ProductController(VehicleRepairDbContext context)
        {
            _context = context;
        }

        //public IActionResult Index()
        //{
        //    var products = _context.Products.ToList();
        //    return View(products);
        //}

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStock(int productId, int newStock)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            product.Stock = newStock;
            _context.Update(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                 _context.Products.Add(product);
                 _context.SaveChanges();
                TempData["SuccessMessage"] = "Ürün başarıyla eklendi.";
                return RedirectToAction("Create");
            }
            TempData["Error Message"] = "Ürün eklenemedi!";
            return View(product);
        }
    }
}
