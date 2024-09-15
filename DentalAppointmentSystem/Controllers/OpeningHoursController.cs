using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DentalAppointmentSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DentalAppointmentSystem.Controllers
{
    public class OpeningHoursController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OpeningHoursController(ApplicationDbContext context)
        {
            _context = context;
        }

        // عرض أوقات العمل فقط على الموقع
        [HttpGet]
        //public async Task<IActionResult> Index()
        //{
        //    var openingHours = await _context.OpeningHours.Include(o => o.Dentist).ToListAsync();
        //    return View(openingHours);  // صفحة الموقع لعرض أوقات العمل فقط
        //}

        // عرض أوقات العمل في لوحة التحكم
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var openingHours = await _context.OpeningHours.Include(o => o.Dentist).ToListAsync();
            return View(openingHours);  // صفحة الداشبورد لعرض أوقات العمل
        }

        // عرض التفاصيل في الداشبورد
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var openingHour = await _context.OpeningHours
                .Include(o => o.Dentist)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (openingHour == null) return NotFound();

            return View(openingHour);
        }

        // إضافة أوقات عمل جديدة
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["DentistId"] = new SelectList(_context.Dentists, "ID", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Day,From,To,From2,To2,DentistId")] OpeningHours openingHours)
        {
            if (ModelState.IsValid)
            {
                _context.Add(openingHours);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Dashboard));
            }
            ViewData["DentistId"] = new SelectList(_context.Dentists, "ID", "Name", openingHours.DentistId);
            return View(openingHours);
        }

        // تعديل أوقات العمل
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var openingHour = await _context.OpeningHours.FindAsync(id);
            if (openingHour == null) return NotFound();

            ViewData["DentistId"] = new SelectList(_context.Dentists, "ID", "Name", openingHour.DentistId);
            return View(openingHour);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Day,From,To,From2,To2,DentistId")] OpeningHours openingHours)
        {
            if (id != openingHours.ID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(openingHours);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OpeningHoursExists(openingHours.ID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Dashboard));
            }
            ViewData["DentistId"] = new SelectList(_context.Dentists, "ID", "Name", openingHours.DentistId);
            return View(openingHours);
        }

        // حذف أوقات العمل
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var openingHour = await _context.OpeningHours
                .Include(o => o.Dentist)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (openingHour == null) return NotFound();

            return View(openingHour);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var openingHour = await _context.OpeningHours.FindAsync(id);
            if (openingHour != null)
            {
                _context.OpeningHours.Remove(openingHour);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Dashboard));
        }

        // البحث عن أوقات العمل في الداشبورد
        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            var result = await _context.OpeningHours
                .Include(o => o.Dentist)
                .Where(o => o.Day.Contains(query) || o.Dentist.Name.Contains(query))
                .ToListAsync();
            return View("Dashboard", result);  // عرض نتائج البحث في الداشبورد
        }

        // دالة البحث عن الدكاترة المتواجدين حسب التاريخ والخدمة
        [HttpGet]
        public async Task<IActionResult> AvailableDentists(DateTime date, int serviceId)
        {
            var availableDentists = await _context.OpeningHours
                .Include(o => o.Dentist)
                .Where(o => o.From <= date.TimeOfDay && o.To >= date.TimeOfDay && o.Dentist.ServerId == serviceId)
                .Select(o => o.Dentist)
                .Distinct()
                .ToListAsync();

            return View(availableDentists);  // عرض الدكاترة المتواجدين
        }

        // دالة التحقق من وجود أوقات العمل
        private bool OpeningHoursExists(int id)
        {
            return _context.OpeningHours.Any(e => e.ID == id);
        }


        

        //// عرض صفحة Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        // عرض صفحة Edit
        public IActionResult Edit(int id)
        {
            var openingHour = _context.OpeningHours.Find(id);
            if (openingHour == null)
            {
                return NotFound();
            }
            return View(openingHour);
        }

        // عرض صفحة Delete
        public IActionResult Delete(int id)
        {
            var openingHour = _context.OpeningHours.Find(id);
            if (openingHour == null)
            {
                return NotFound();
            }
            return View(openingHour);
        }
    }
}
