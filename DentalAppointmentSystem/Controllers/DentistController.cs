using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DentalAppointmentSystem.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DentalAppointmentSystem.Controllers
{
    public class DentistController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DentistController(ApplicationDbContext context)
        {
            _context = context;
        }

        // For the main page - display only
        public async Task<IActionResult> Index()
        {
            var dentists = await _context.Dentists
                                         .Include(d => d.Server)
                                         .ToListAsync();
            return View(dentists); // Display list of dentists on the main page
        }

        // Dashboard: Display and search functionality
        public async Task<IActionResult> DashboardIndex(string searchString)
        {
            var dentists = _context.Dentists.Include(d => d.Server).AsQueryable();

            // Search functionality
            if (!String.IsNullOrEmpty(searchString))
            {
                dentists = dentists.Where(d =>
                    d.Name.Contains(searchString) ||
                    d.Specialization.Contains(searchString) ||
                    d.Email.Contains(searchString) ||
                    d.Phone.Contains(searchString));
            }

            return View(await dentists.ToListAsync());
        }

        // GET: Dentist/Details/5 (View details of a specific dentist)
        public async Task<IActionResult> Details(int id)
        {
            var dentist = await _context.Dentists
                .Include(d => d.Server)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (dentist == null)
            {
                return NotFound();
            }

            return View(dentist);
        }

        //[HttpGet]
        //public IActionResult Create()
        //{
        //    ViewData["ServerId"] = new SelectList(_context.Services, "ID", "Name");
        //    return View();
        //}

        // POST: Dentist/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Dentist dentist, IFormFile Image)
        {
            try
            {
               // if (ModelState.IsValid)
                {
                    // التحقق من أن هناك صورة تم تحميلها
                    if (Image != null && Image.Length > 0)
                    {
                        // توليد اسم فريد للصورة باستخدام GUID
                        var imageFileName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);

                        // تحديد المسار الذي سيتم حفظ الصورة فيه داخل مجلد wwwroot
                        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", imageFileName);

                        // حفظ الصورة في المجلد المحدد
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await Image.CopyToAsync(stream);
                        }

                        // حفظ مسار الصورة في قاعدة البيانات
                        dentist.Image = "/img/" + imageFileName;
                    }

                    // إضافة بيانات الطبيب إلى قاعدة البيانات
                    _context.Add(dentist);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("AdminIndex", "Dentist");
                }

                // في حال كان هناك خطأ في النموذج نحتاج إلى إعادة تحميل القائمة
                ViewData["ServerId"] = new SelectList(_context.Services, "ID", "Name", dentist.ServerId);
                return View(dentist);
            }
            catch
            {
                ViewData["ServerId"] = new SelectList(_context.Services, "ID", "Name", dentist.ServerId);
                return View(dentist);
            }
        }


        //// GET: Dentist/Edit/5
        //public async Task<IActionResult> Edit(int id)
        //{
        //    var dentist = await _context.Dentists.FindAsync(id);
        //    if (dentist == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(dentist);
        //}

        // POST: Dentist/Edit/5
        // POST: Dentist/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Specialization,Description,Image,Phone,Email,X,Facebook,LinkedIn,Instagram,ServerId")] Dentist dentist)
        {
            if (id != dentist.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dentist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DentistExists(dentist.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(DashboardIndex));
            }

            // إعادة تحميل القائمة في حالة فشل الحفظ
            ViewData["ServerId"] = new SelectList(_context.Services, "ID", "Name", dentist.ServerId);
            return View(dentist);
        }

        // GET: Dentist/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var dentist = await _context.Dentists
                .FirstOrDefaultAsync(m => m.ID == id);

            if (dentist == null)
            {
                return NotFound();
            }

            return View(dentist);
        }

        // POST: Dentist/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dentist = await _context.Dentists.FindAsync(id);
            if (dentist != null)
            {
                _context.Dentists.Remove(dentist);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(DashboardIndex));
        }

        private bool DentistExists(int id)
        {
            return _context.Dentists.Any(e => e.ID == id);
        }
    }
}
