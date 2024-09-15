using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DentalAppointmentSystem.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DentalAppointmentSystem.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST-only for the main website (saving contact form)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(Contact contact)
        {
            if (ModelState.IsValid)
            {
                contact.SentAt = DateTime.Now;
                _context.Contacts.Add(contact);
                await _context.SaveChangesAsync();
                return RedirectToAction("Confirmation"); // Redirect to confirmation page or action
            }

            return View(contact); // If validation fails, return to the same view with errors
        }

        // Confirmation page after successful contact form submission
        public IActionResult Confirmation()
        {
            return View();
        }

        // Display and search functionality for the dashboard

        // GET: Contact (Display all contacts with optional search)
        public async Task<IActionResult> Index(string searchString)
        {
            var contacts = _context.Contacts.AsQueryable();

            // Implement search functionality
            if (!String.IsNullOrEmpty(searchString))
            {
                contacts = contacts.Where(c =>
                    c.Name.Contains(searchString) ||
                    c.Subject.Contains(searchString) ||
                    c.Email.Contains(searchString));
            }

            return View(await contacts.ToListAsync());
        }

        // GET: Contact/Details/5 (View details of a specific contact)
        public async Task<IActionResult> Details(int id)
        {
            var contact = await _context.Contacts
                .FirstOrDefaultAsync(m => m.ID == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }
    }
}
