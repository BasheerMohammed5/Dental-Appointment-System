using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DentalAppointmentSystem.Models;

namespace DentalAppointmentSystem.Controllers
{
    public class PricesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PricesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // عرض الأسعار في الصفحة الرئيسية (فقط العرض)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var prices = await _context.Prices
                .Include(p => p.Server)
                .ToListAsync();
            return View(prices);
        }

        // عرض الأسعار في لوحة التحكم (Dashboard)
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var prices = await _context.Prices
                .Include(p => p.Server)
                .ToListAsync();
            return View(prices);
        }

        // إضافة سعر جديد (عرض صفحة الإضافة)
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Servers"] = _context.Services.ToList();  // تحميل قائمة الخدمات المتاحة
            return View();
        }

        // إضافة سعر جديد (تنفيذ عملية الحفظ)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ServerId,Description,Price")] Prices prices)
        {
            if (ModelState.IsValid)
            {
                _context.Add(prices);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Dashboard));
            }
            ViewData["Servers"] = _context.Services.ToList();
            return View(prices);
        }

        // تعديل سعر (عرض صفحة التعديل)
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var price = await _context.Prices.FindAsync(id);
            if (price == null) return NotFound();

            ViewData["Servers"] = _context.Services.ToList();  // تحميل قائمة الخدمات المتاحة
            return View(price);
        }

        // تعديل سعر (تنفيذ عملية الحفظ)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ServerId,Description,Price")] Prices prices)
        {
            if (id != prices.ID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prices);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PricesExists(prices.ID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Dashboard));
            }
            ViewData["Servers"] = _context.Services.ToList();
            return View(prices);
        }

        // حذف سعر (عرض صفحة التأكيد)
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var price = await _context.Prices
                .Include(p => p.Server)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (price == null) return NotFound();

            return View(price);
        }

        // حذف سعر (تنفيذ عملية الحذف)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var price = await _context.Prices.FindAsync(id);
            _context.Prices.Remove(price);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Dashboard));
        }

        // البحث عن سعر معين
        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            var result = await _context.Prices
                .Where(p => p.Description.Contains(query) || p.Server.Name.Contains(query)) // البحث في الوصف أو الخدمة
                .Include(p => p.Server)
                .ToListAsync();

            return View("Dashboard", result);  // عرض نتائج البحث في لوحة التحكم
        }

        // التحقق من وجود سعر معين
        private bool PricesExists(int id)
        {
            return _context.Prices.Any(e => e.ID == id);
        }
    }
}
