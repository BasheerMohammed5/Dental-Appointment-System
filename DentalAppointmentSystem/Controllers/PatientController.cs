using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DentalAppointmentSystem.Models;

namespace DentalAppointmentSystem.Controllers
{
    public class PatientController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientController(ApplicationDbContext context)
        {
            _context = context;
        }

        // عرض المرضى في الداشبورد
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var patients = await _context.Patients.Include(p => p.Appointments).ToListAsync();
            return View(patients);  // صفحة الداشبورد لعرض بيانات المرضى
        }

        // عرض تفاصيل مريض معين
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var patient = await _context.Patients
                .Include(p => p.Appointments)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (patient == null) return NotFound();

            return View(patient);
        }

        // إضافة مريض جديد
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Phone,Email,DateOfBirth,Gender")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Dashboard));
            }
            return View(patient);
        }

        // تعديل بيانات مريض
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return NotFound();

            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Phone,Email,DateOfBirth,Gender")] Patient patient)
        {
            if (id != patient.ID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.ID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Dashboard));
            }
            return View(patient);
        }

        // حذف مريض
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var patient = await _context.Patients
                .FirstOrDefaultAsync(m => m.ID == id);
            if (patient == null) return NotFound();

            return View(patient);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Dashboard));
        }

        // البحث عن المرضى في الداشبورد
        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            var result = await _context.Patients
                .Where(p => p.Name.Contains(query) || p.Phone.Contains(query) || p.Email.Contains(query))
                .Include(p => p.Appointments)
                .ToListAsync();

            return View("Dashboard", result);  // عرض نتائج البحث في الداشبورد
        }

        // التحقق من وجود مريض
        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.ID == id);
        }
    }
}
