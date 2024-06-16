using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleRepairMVC.Context;
using VehicleRepairMVC.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Layout.Properties;

namespace VehicleRepairMVC.Controllers
{
    public class ProfileController : Controller
    {
        private readonly VehicleRepairDbContext _context;

        public ProfileController(VehicleRepairDbContext context)
        {
            _context = context;
        }

        public IActionResult MyAppointments()
        {
            ViewBag.IsLoggedIn = HttpContext.Session.GetString("IsLoggedIn") == "true";
            if (HttpContext.Session.GetInt32("LoggedInUserId").HasValue)
            {
                int? loggedInUserId = HttpContext.Session.GetInt32("LoggedInUserId");
                if (loggedInUserId.HasValue)
                {
                    var approvedAppointments = _context.Appointments
                        .Include(a => a.Car)
                        .Where(a => a.Car.UserID == loggedInUserId.Value && (a.Status == "Onaylandı" || a.Status == "İptal Edildi"))
                        .ToList();

                    return View(approvedAppointments);
                }
            }

            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public async Task<IActionResult> CancelAppointment(int appointmentId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
            {
                return NotFound();
            }

            // Randevu tarihinden şu anki zamanı çıkararak fark oluştur
            var timeDifference = appointment.AppointmentDate - DateTime.Now;

            // Eğer randevu tarihi geçmişse veya fark 24 saatten az ise, randevuyu iptal etmeye izin verme
            if (timeDifference.TotalHours < 24)
            {
                TempData["ErrorMessage"] = "Randevuyu son 24 saat içinde iptal edemezsiniz!";
                return RedirectToAction("MyAppointments"); // Hata mesajıyla birlikte MyAppointments sayfasına yönlendir
            }

            // Randevuyu iptal etme işlemi
            appointment.Status = "İptal Edildi"; // Statusu güncelle
            await _context.SaveChangesAsync();

            return RedirectToAction("MyAppointments"); // Silme işlemi tamamlandıktan sonra MyAppointments sayfasına yönlendir.
        }

        public IActionResult MyVehicles()
        {
            ViewBag.IsLoggedIn = HttpContext.Session.GetString("IsLoggedIn") == "true";
            if (HttpContext.Session.GetInt32("LoggedInUserId").HasValue)
            {
                int? loggedInUserId = HttpContext.Session.GetInt32("LoggedInUserId");
                if (loggedInUserId.HasValue)
                {
                    var userCars = _context.Cars.Where(c => c.UserID == loggedInUserId.Value).ToList();
                    return View(userCars);
                }
            }
            return RedirectToAction("Index", "Login");
        }

        public async Task<IActionResult> MyInvoice(int appointmentId)
        {
            ViewBag.IsLoggedIn = HttpContext.Session.GetString("IsLoggedIn") == "true";
            if (HttpContext.Session.GetInt32("LoggedInUserId").HasValue)
            {
                int? loggedInUserId = HttpContext.Session.GetInt32("LoggedInUserId");
                if (loggedInUserId.HasValue)
                {
                    // Kullanıcının onaylanmış ve iptal edilmiş randevularını al
                    var approvedAppointments = _context.Appointments
                        .Include(a => a.Car)
                        .ThenInclude(c => c.User) // İlişkili User tablosunu da dahil et
                        .Include(a => a.RepairProcesses) // İlişkili RepairProcess tablosunu da dahil et
                        .Where(a => a.Car.UserID == loggedInUserId.Value &&
                                    (a.Status == "Onaylandı" || a.Status == "İptal Edildi"))
                        .ToList();

                    return View(approvedAppointments);
                }
            }

            return RedirectToAction("Index", "Login");
        }


        public async Task<IActionResult> PrintInvoice(int appointmentId)
        {
            ViewBag.IsLoggedIn = HttpContext.Session.GetString("IsLoggedIn") == "true";
            // Oturum açmış kullanıcının kimliğini al
            int? loggedInUserId = HttpContext.Session.GetInt32("LoggedInUserId");

            if (loggedInUserId.HasValue)
            {
                var appointment = await _context.Appointments
                    .Include(a => a.Car)
                    .ThenInclude(c => c.User)
                    .Include(a => a.RepairProcesses)
                    .FirstOrDefaultAsync(a => a.AppointmentID == appointmentId && a.Car.UserID == loggedInUserId.Value);

                if (appointment == null)
                {
                    return NotFound();
                }

                var invoiceModel = new InvoiceViewModel
                {
                    UserName = appointment.Car.User.Username,
                    UserFullName = $"{appointment.Car.User.Firstname} {appointment.Car.User.Lastname}",
                    LicensePlate = appointment.Car.LicensePlate,
                    AppointmentDate = appointment.AppointmentDate,
                    RepairProcesses = appointment.RepairProcesses.ToList(),
                    TotalPrice = appointment.RepairProcesses.Sum(rp => rp.ProcessPrice)
                };

                using (var memoryStream = new MemoryStream())
                {
                    var writer = new PdfWriter(memoryStream);
                    var pdf = new PdfDocument(writer);
                    var document = new Document(pdf);

                    var fontPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fonts", "arial.ttf");
                    var pdfFont = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H);
                    document.SetFont(pdfFont);

                    document.Add(new Paragraph("Fatura")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(20)
                        .SetFont(pdfFont));

                    document.Add(new Paragraph($"Kullanıcı Adı: {invoiceModel.UserName}").SetFont(pdfFont));
                    document.Add(new Paragraph($"Ad Soyad: {invoiceModel.UserFullName}").SetFont(pdfFont));

                    document.Add(new Paragraph(""));

                    Table table = new Table(3, true);
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Randevu Tarihi").SetFont(pdfFont)).SetTextAlignment(TextAlignment.CENTER));
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Yapılan İşlem").SetFont(pdfFont)).SetTextAlignment(TextAlignment.CENTER));
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Tutar").SetFont(pdfFont)).SetTextAlignment(TextAlignment.CENTER));

                    foreach (var repairProcess in invoiceModel.RepairProcesses)
                    {
                        table.AddCell(new Cell().Add(new Paragraph(appointment.AppointmentDate.ToString("dd/MM/yyyy")).SetFont(pdfFont)).SetTextAlignment(TextAlignment.CENTER));
                        table.AddCell(new Cell().Add(new Paragraph(repairProcess.RepairProcessDescription).SetFont(pdfFont)).SetTextAlignment(TextAlignment.CENTER));
                        table.AddCell(new Cell().Add(new Paragraph(repairProcess.ProcessPrice.ToString("C")).SetFont(pdfFont)).SetTextAlignment(TextAlignment.CENTER));
                    }
                    document.Add(table);

                    document.Add(new Paragraph($"Toplam Tutar: {invoiceModel.TotalPrice.ToString("C")}")
         .SetTextAlignment(TextAlignment.RIGHT)
         .SetFont(pdfFont));


                    
                    document.Close();

                    byte[] pdfBytes = memoryStream.ToArray();
                    return File(pdfBytes, "application/pdf", "Fatura.pdf");
                }
            }

            return RedirectToAction("Index", "Login");
        }
    }
        }


