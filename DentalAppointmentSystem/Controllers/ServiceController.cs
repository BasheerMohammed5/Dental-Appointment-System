using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using DentalAppointmentSystem.Models;

namespace DentalAppointmentSystem.Controllers
{
    public class ServiceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServiceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // عرض الخدمات في الصفحة الرئيسية
        [HttpGet("Admin/Service/Index")]
        public async Task<IActionResult> Index()
        {
            var services = await _context.Services.ToListAsync();
            return View(services);
        }

        // عرض الخدمات في لوحة التحكم (Dashboard)
        [HttpGet("Admin/Service/Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var services = await _context.Services.ToListAsync();
            return View(services);
        }

        //// إضافة خدمة جديدة (عرض صفحة الإضافة)
        //[HttpGet("Admin/Service/Create")]
        //public IActionResult Create()
        //{
        //    return View();
        //}

        // إضافة خدمة جديدة مع رفع الصور (تنفيذ عملية الحفظ)
        [HttpPost("Admin/Service/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Service service, IFormFile Images)
        {
            //if (ModelState.IsValid)
            {
                if (Images != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(Images.FileName);
                    string extension = Path.GetExtension(Images.FileName);
                    service.Images = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", service.Images);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await Images.CopyToAsync(fileStream);
                    }
                }

                _context.Add(service);
                await _context.SaveChangesAsync();
                return RedirectToAction("~/Admin/Service/AdminIndex");
            }
            return View(service);
        }

        //// تعديل خدمة (عرض صفحة التعديل)
        //[HttpGet("Admin/Service/Edit/{id}")]
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null) return NotFound();

        //    var service = await _context.Services.FindAsync(id);
        //    if (service == null) return NotFound();

        //    return View(service);
        //}

        // تعديل خدمة مع رفع الصور (تنفيذ عملية الحفظ)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Service/EditService")]
        public async Task<IActionResult> EditService(int id, Service service, IFormFile Images)
        {
            if (id != service.ID) return NotFound();

            //if (ModelState.IsValid)
            {
                if (Images != null)
                {
                    // حذف الصورة القديمة
                    string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", service.Images);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }

                    // رفع الصورة الجديدة
                    string fileName = Path.GetFileNameWithoutExtension(Images.FileName);
                    string extension = Path.GetExtension(Images.FileName);
                    service.Images = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", service.Images);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await Images.CopyToAsync(fileStream);
                    }
                }

                try
                {
                    _context.Update(service);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceExists(service.ID)) return NotFound();
                    else throw;
                }
                //return RedirectToAction("AdminIndex", "Service");
                return Redirect("https://localhost:7247/Admin/Service/Adminlndex.cshtml");

            }
            return View(service);
        }


        // حذف خدمة (عرض صفحة التأكيد)
        [HttpGet("Admin/Service/Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var service = await _context.Services.FirstOrDefaultAsync(m => m.ID == id);
            if (service == null) return NotFound();

            return View(service);
        }

        // حذف خدمة (تنفيذ عملية الحذف)
        [HttpPost("Admin/Service/Delete/{id}")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service.Images != null)
            {
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", service.Images);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
            return RedirectToAction("Dashboard");
        }

        // البحث عن خدمة معينة
        [HttpGet("Admin/Service/Search")]
        public async Task<IActionResult> Search(string query)
        {
            var result = await _context.Services
                .Where(s => s.Name.Contains(query) || s.Description.Contains(query))
                .ToListAsync();

            return View("Dashboard", result);
        }

        private bool ServiceExists(int id)
        {
            return _context.Services.Any(e => e.ID == id);
        }
    }
}
