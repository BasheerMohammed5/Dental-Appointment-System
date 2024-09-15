using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DentalAppointmentSystem.Models;

namespace DentalAppointmentSystem.Controllers
{
    public class WaitingListController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WaitingListController(ApplicationDbContext context)
        {
            _context = context;
        }

        // عرض جميع السجلات
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var waitingList = await _context.WaitingList
                .Include(w => w.Server)
                .Include(w => w.Dentist)
                .ToListAsync();
            return View(waitingList);
        }

        // عرض صفحة إضافة سجل جديد
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // حفظ سجل جديد
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ServerId,DentistId,PatientName,PhoneNumber,EmailAddress,PreferredDate")] WaitingList waitingList)
        {
            if (ModelState.IsValid)
            {
                _context.Add(waitingList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(waitingList);
        }

        // عرض صفحة تعديل سجل موجود
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var waitingList = await _context.WaitingList.FindAsync(id);
            if (waitingList == null) return NotFound();

            return View(waitingList);
        }

        // تعديل سجل موجود
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ServerId,DentistId,PatientName,PhoneNumber,EmailAddress,PreferredDate,IsNotified")] WaitingList waitingList)
        {
            if (id != waitingList.ID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(waitingList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WaitingListExists(waitingList.ID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(waitingList);
        }

        // عرض صفحة تأكيد حذف سجل
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var waitingList = await _context.WaitingList
                .Include(w => w.Server)
                .Include(w => w.Dentist)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (waitingList == null) return NotFound();

            return View(waitingList);
        }

        // حذف سجل
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var waitingList = await _context.WaitingList.FindAsync(id);
            _context.WaitingList.Remove(waitingList);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // البحث في قائمة الانتظار
        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            var result = await _context.WaitingList
                .Include(w => w.Server)
                .Include(w => w.Dentist)
                .Where(w => w.PatientName.Contains(query) || w.PhoneNumber.Contains(query))
                .ToListAsync();

            return View("Index", result);
        }

        // التحقق من وجود السجل
        private bool WaitingListExists(int id)
        {
            return _context.WaitingList.Any(e => e.ID == id);
        }
    }
}
