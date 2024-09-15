using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DentalAppointmentSystem.Models;
using static System.Net.Mime.MediaTypeNames;

namespace DentalAppointmentSystem.Controllers
{
    public class TestimonialController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TestimonialController(ApplicationDbContext context)
        {
            _context = context;
        }

        // عرض التقييمات في الموقع (فقط العرض)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var testimonials = await _context.Testimonials.ToListAsync();
            return View(testimonials);
        }

        // عرض التقييمات في لوحة التحكم (Dashboard)
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var testimonials = await _context.Testimonials.ToListAsync();
            return View(testimonials);
        }

        // إضافة تقييم جديد (عرض صفحة الإضافة)
        //[HttpGet]
        //public IActionResult Create()
        //{
        //    return View();
        //}

        // إضافة تقييم جديد (تنفيذ عملية الحفظ)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Testimonial testimonial, IFormFile Images)
        {
           // if (ModelState.IsValid)
            {
                if (Images != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(Images.FileName);
                    string extension = Path.GetExtension(Images.FileName);
                    testimonial.Image = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", testimonial.Image);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await Images.CopyToAsync(fileStream);
                    }
                }

                _context.Add(testimonial);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Dashboard));
            }
            return View(testimonial);
        }

        // تعديل تقييم (عرض صفحة التعديل)
        //[HttpGet]
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null) return NotFound();

        //    var testimonial = await _context.Testimonials.FindAsync(id);
        //    if (testimonial == null) return NotFound();

        //    return View(testimonial);
        //}

        // تعديل تقييم (تنفيذ عملية الحفظ)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Testimonial testimonial, IFormFile Images)
        {
            if (id != testimonial.ID) return NotFound();

            //if (ModelState.IsValid)
            {
                try
                {
                    if (Images != null)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(Images.FileName);
                        string extension = Path.GetExtension(Images.FileName);
                        testimonial.Image = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", testimonial.Image);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await Images.CopyToAsync(fileStream);
                        }
                    }


                    _context.Update(testimonial);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TestimonialExists(testimonial.ID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Dashboard));
            }
            return View(testimonial);
        }

        // حذف تقييم (عرض صفحة التأكيد)
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var testimonial = await _context.Testimonials
           .FirstOrDefaultAsync(m => m.ID == id);
            if (testimonial == null) return NotFound();

            return View(testimonial);
        }

        // حذف تقييم (تنفيذ عملية الحذف)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var testimonial = await _context.Testimonials.FindAsync(id);
            _context.Testimonials.Remove(testimonial);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Dashboard));
        }

        // البحث عن تقييم معين
        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            var result = await _context.Testimonials
                .Where(t => t.Name.Contains(query) || t.Text.Contains(query)) // البحث في الاسم أو النص
                .ToListAsync();

            return View("Dashboard", result);  // عرض نتائج البحث في لوحة التحكم
        }

        // التحقق من وجود تقييم معين
        private bool TestimonialExists(int id)
        {
            return _context.Testimonials.Any(e => e.ID == id);
        }
    }
}
