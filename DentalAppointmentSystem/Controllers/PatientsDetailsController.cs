using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DentalAppointmentSystem.Models;

namespace DentalAppointmentSystem.Controllers
{
    public class PatientsDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientsDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // عرض جميع بيانات المرضى
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var patientDetails = await _context.PatientDetails
                .Include(pd => pd.Patient)
                .Include(pd => pd.Server)
                .ToListAsync();
            return View(patientDetails);
        }

        // عرض تفاصيل مريض معين
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var patientDetail = await _context.PatientDetails
                .Include(pd => pd.Patient)
                .Include(pd => pd.Server)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (patientDetail == null) return NotFound();

            return View(patientDetail);
        }

        // إضافة بيانات مريض جديد
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Patients"] = _context.Patients.ToList();  // تحميل قائمة المرضى
            ViewData["Servers"] = _context.Services.ToList();   // تحميل قائمة الخدمات
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,PatientID,RegisterNumber,ServerId,Address,Date,ReturnDate,Notes,files,BloodType,Allergies")] PatientDetails patientDetails)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patientDetails);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Patients"] = _context.Patients.ToList();
            ViewData["Servers"] = _context.Services.ToList();
            return View(patientDetails);
        }

        // تعديل بيانات مريض
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var patientDetail = await _context.PatientDetails.FindAsync(id);
            if (patientDetail == null) return NotFound();

            ViewData["Patients"] = _context.Patients.ToList();
            ViewData["Servers"] = _context.Services.ToList();
            return View(patientDetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,PatientID,RegisterNumber,ServerId,Address,Date,ReturnDate,Notes,files,BloodType,Allergies")] PatientDetails patientDetails)
        {
            if (id != patientDetails.ID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patientDetails);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientDetailsExists(patientDetails.ID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Patients"] = _context.Patients.ToList();
            ViewData["Servers"] = _context.Services.ToList();
            return View(patientDetails);
        }

        // حذف بيانات مريض
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var patientDetail = await _context.PatientDetails
                .Include(pd => pd.Patient)
                .Include(pd => pd.Server)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (patientDetail == null) return NotFound();

            return View(patientDetail);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patientDetail = await _context.PatientDetails.FindAsync(id);
            _context.PatientDetails.Remove(patientDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // البحث عن بيانات مريض
        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            var result = await _context.PatientDetails
                .Where(pd => pd.Patient.Name.Contains(query) || pd.BloodType.Contains(query) || pd.Allergies.Contains(query))
                .Include(pd => pd.Patient)
                .Include(pd => pd.Server)
                .ToListAsync();

            return View("Index", result);  // عرض نتائج البحث
        }

        // التحقق من وجود بيانات مريض
        private bool PatientDetailsExists(int id)
        {
            return _context.PatientDetails.Any(e => e.ID == id);
        }
    }
}
