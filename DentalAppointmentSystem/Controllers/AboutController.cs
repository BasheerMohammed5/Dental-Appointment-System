using Microsoft.AspNetCore.Mvc;

namespace DentalAppointmentSystem.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
