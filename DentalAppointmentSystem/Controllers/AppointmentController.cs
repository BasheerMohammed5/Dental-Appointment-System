using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DentalAppointmentSystem.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DentalAppointmentSystem.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST-only for the website (Booking appointments)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Book(Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                // Generate a unique booking code
                appointment.BookingCode = GenerateBookingCode();
                appointment.Status = AppointmentStatus.New;
                appointment.CreatedAt = DateTime.Now;

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                return RedirectToAction("Confirmation", new { id = appointment.ID });
            }

            return View(appointment); // Redisplay the form in case of errors
        }

        // Display confirmation of appointment booking
        public ActionResult Confirmation(int id)
        {
            var appointment = _context.Appointments.Find(id);
            if (appointment == null)
            {
                return NotFound();
            }
            return View(appointment);
        }

        // Full CRUD operations for the dashboard

        // GET: Appointment (List all appointments)
        public async Task<IActionResult> Index(string searchString)
        {
            var appointments = _context.Appointments
                                       .Include(a => a.Dentist)
                                       .Include(a => a.Server);

            // Search functionality
            if (!String.IsNullOrEmpty(searchString))
            {
                appointments = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Appointment, Service>)appointments.Where(a =>
                    a.PatientName.Contains(searchString) ||
                    a.Phone.Contains(searchString) ||
                    a.Email.Contains(searchString) ||
                    a.Dentist.Name.Contains(searchString) || // Assuming Dentist has a Name property
                    a.Server.Name.Contains(searchString) // Assuming Server has a Name property
                );
            }

            var openingHours = await _context.Services
                .OrderBy(o => o.Name)
                .ToListAsync();

            var dentists = await _context.Dentists
                .OrderBy(d => d.Name)
                .ToListAsync();

            // تمرير البيانات إلى View باستخدام ViewData
            ViewData["Services"] = openingHours;
            ViewData["Dentist"] = dentists;


            //return View(await appointments.ToListAsync());
            return View();
        }

        // GET: Appointment/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(m => m.ID == id);
            if (appointment == null)
            {
                return NotFound();
            }
            return View(appointment);
        }

        //// GET: Appointment/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        // POST: Appointment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            //if (ModelState.IsValid)
            {
                appointment.BookingCode = GenerateBookingCode();
                appointment.Status = AppointmentStatus.New;
                appointment.CreatedAt = DateTime.Now;

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(appointment);
        }



        // GET: Appointment/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            return View(appointment);
        }

        // POST: Appointment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,Appointment appointment)
        {
            if (id != appointment.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(appointment);
        }

        // GET: Appointment/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(m => m.ID == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Appointment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.ID == id);
        }

        // Helper method to generate a unique booking code
        private string GenerateBookingCode()
        {
           // return Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

            // This function generates a random string of 8 characters (letters and digits)
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
