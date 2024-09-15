using DentalAppointmentSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DentalAppointmentSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Ã·» √Êﬁ«  «·⁄„·
            var openingHours = await _context.OpeningHours
                .OrderBy(o => o.Day)
                .ToListAsync();

            // Ã·» ﬁ«∆„… «·œﬂ« —…
            var dentists = await _context.Dentists
                .OrderBy(d => d.Name)
                .ToListAsync();

            //  „—Ì— «·»Ì«‰«  ≈·Ï View »«” Œœ«„ ViewData
            ViewData["OpeningHours"] = openingHours;
            ViewData["Dentist"] = dentists;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
