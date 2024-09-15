using DentalAppointmentSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

[Area("Admin")]
//[Authorize] // يمكنك تفعيل هذا عند الحاجة للحماية
public class DashboardController : Controller
{
    private ApplicationDbContext _context;


    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }
    // الأكشن لعرض الصفحة الرئيسية للـ Dashboard
    public IActionResult Index()
    {
        return View("~/Views/Admin/Dashboard/Index.cshtml");
    }

    // الأكشن لعرض صفحة الخطأ 404
    public IActionResult Page404()
    {
        return View("~/Views/Admin/Page404/Index.cshtml");
    }

    //--------------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------OpeningHours

    // عرض خاص بالادمين
    [Area("Admin")]
    [HttpGet("/Admin/OpeningHours/AdminIndex")]
    public IActionResult AdminOpeningHours()
    {
        var openingHours = _context.OpeningHours.ToList();
        return View("~/Views/Admin/OpeningHours/AdminIndex.cshtml", openingHours); // مسار العرض الكامل
    }

    [HttpGet("/Admin/OpeningHours/Create")]
    public IActionResult Create()
    {
        ViewData["DentistId"] = new SelectList(_context.Dentists, "ID", "Name");

        return View("~/Views/Admin/OpeningHours/Create.cshtml"); // سيعرض صفحة Create
    }

    [HttpGet("/Admin/OpeningHours/Edit")]
     public async Task<IActionResult> EditOpeningHours(int? id)
    {
        if (id == null) return NotFound();
        var temp = await _context.OpeningHours.FindAsync(id);
        if (temp == null) return NotFound();

        ViewData["DentistId"] = new SelectList(_context.Dentists, "ID", "Name");

        return View("~/Views/Admin/OpeningHours/Edit.cshtml",temp); // 
    }

    [HttpGet("/Admin/OpeningHours/Delete")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var temp = await _context.OpeningHours.FirstOrDefaultAsync(m => m.ID == id);
        if (temp == null) return NotFound();

        return View("~/Views/Admin/OpeningHours/Delete.cshtml",temp); // 
    }
    //--------------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------Service

    [Area("Admin")]
    [HttpGet("/Admin/Service/AdminIndex")]
    public IActionResult Service()
    {
        var service = _context.Services.ToList();
        return View("~/Views/Admin/Service/AdminIndex.cshtml", service); 
    }

    [HttpGet("/Admin/Service/Create")]
    public IActionResult CreateService()
    {
        return View("~/Views/Admin/Service/Create.cshtml"); 
    }

    [HttpGet("/Admin/Service/EditService")]
    public async Task<IActionResult> EditService(int? id)
    {
        if (id == null) return NotFound();
        var temp = await _context.Services.FindAsync(id);
        if (temp == null) return NotFound();

        return View("~/Views/Admin/Service/Edit.cshtml", temp); // 
    }

    [HttpGet("/Admin/Service/Delete")]
    public async Task<IActionResult> DeleteService(int? id)
    {
        if (id == null) return NotFound();
        var temp = await _context.Services.FirstOrDefaultAsync(m => m.ID == id);
        if (temp == null) return NotFound();

        return View("~/Views/Admin/Service/Delete.cshtml",temp); // 
    }

    //--------------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------Dentist

    [Area("Admin")]
    [HttpGet("/Admin/Dentist/AdminIndex")]
    public IActionResult Dentist()
    {
        var dentist = _context.Dentists.ToList();
        return View("~/Views/Admin/Dentist/AdminIndex.cshtml", dentist);
    }

    [HttpGet("/Admin/Dentist/Create")]
    public IActionResult CreateDentist()
    {
        ViewData["ServerId"] = new SelectList(_context.Services, "ID", "Name");
        return View("~/Views/Admin/Dentist/Create.cshtml"); // 
    }

    [HttpGet("/Admin/Dentist/Edit")]
    public async Task<IActionResult> EditDentist(int? id)
    {
        if (id == null) return NotFound();
        var temp = await _context.Dentists.FindAsync(id);
        if (temp == null) return NotFound();

        ViewData["ServerId"] = new SelectList(_context.Services, "ID", "Name");

        return View("~/Views/Admin/Dentist/Edit.cshtml", temp); // 
    }

    [HttpGet("/Admin/Dentist/Delete")]
    public async Task<IActionResult> DeleteDentist(int? id)
    {
        if (id == null) return NotFound();
        var temp = await _context.Dentists.FirstOrDefaultAsync(m => m.ID == id);
        if (temp == null) return NotFound();

        return View("~/Views/Admin/Service/Delete.cshtml",temp); // 
    }

    //--------------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------Appointment

    [Area("Admin")]
    [HttpGet("/Admin/Appointment/AdminIndex")]
    public IActionResult Appointment(string query)
    {
        var appointments = _context.Appointments
            .Include(a => a.Dentist)
            .Include(a => a.Server)
            .AsQueryable();

        // إذا كان هناك بحث بواسطة query
        if (!string.IsNullOrEmpty(query))
        {
            appointments = appointments.Where(a => a.PatientName.Contains(query)
            || a.Phone.Contains(query)
            || a.Email.Contains(query));
        }

        return View("~/Views/Admin/Appointment/AdminIndex.cshtml", appointments.ToList());
    }


    [HttpGet("/Admin/Appointment/Create")]
    public IActionResult CreateAppointment()
    {
        ViewData["ServerId"] = new SelectList(_context.Services, "ID", "Name");
        ViewData["DentistId"] = new SelectList(_context.Dentists, "ID", "Name");
        return View("~/Views/Admin/Appointment/Create.cshtml"); // 
    }

    [HttpGet("/Admin/Appointment/Edit")]
    public async Task<IActionResult> EditAppointment(int? id)
    {
        if (id == null) return NotFound();
        var temp = await _context.Appointments.FindAsync(id);
        if (temp == null) return NotFound();

        ViewData["ServerId"] = new SelectList(_context.Services, "ID", "Name");
        ViewData["DentistId"] = new SelectList(_context.Dentists, "ID", "Name");

        return View("~/Views/Admin/Appointment/Edit.cshtml", temp); // 
    }

    [HttpGet("/Admin/Appointment/Delete")]
    public async Task<IActionResult> DeleteAppointment(int? id)
    {
        if (id == null) return NotFound();
        var temp = await _context.Appointments.FirstOrDefaultAsync(m => m.ID == id);
        if (temp == null) return NotFound();

        return View("~/Views/Admin/Appointment/Delete.cshtml",temp); // 
    }

    //--------------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------Testimonial

    [Area("Admin")]
    [HttpGet("/Admin/Testimonial/AdminIndex")]
    public IActionResult Testimonial()
    {
        var dentist = _context.Testimonials.ToList();
        return View("~/Views/Admin/Testimonial/AdminIndex.cshtml", dentist);
    }

    [HttpGet("/Admin/Testimonial/Create")]
    public IActionResult CreateTestimonial()
    {
        return View("~/Views/Admin/Testimonial/Create.cshtml"); // 
    }

    [HttpGet("/Admin/Testimonial/Edit")]
    public async Task<IActionResult> EditTestimonial(int? id)
    {
        if (id == null) return NotFound();
        var temp = await _context.Testimonials.FindAsync(id);
        if (temp == null) return NotFound();

        return View("~/Views/Admin/Testimonial/Edit.cshtml", temp); // 
    }

    [HttpGet("/Admin/Testimonial/Delete")]
    public async Task<IActionResult> DeleteTestimonial(int? id)
    {
        if (id == null) return NotFound();
        var temp = await _context.Testimonials.FirstOrDefaultAsync(m => m.ID == id);
        if (temp == null) return NotFound();

        return View("~/Views/Admin/Testimonial/Delete.cshtml", temp); // 
    }

    //--------------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------Prices

    [Area("Admin")]
    [HttpGet("/Admin/Prices/AdminIndex")]
    public IActionResult Prices()
    {
        var dentist = _context.Prices.ToList();
        return View("~/Views/Admin/Prices/AdminIndex.cshtml", dentist);
    }

    [HttpGet("/Admin/Prices/Create")]
    public IActionResult CreatePrices()
    {
        ViewData["DentistId"] = new SelectList(_context.Dentists, "ID", "Name");
        return View("~/Views/Admin/Prices/Create.cshtml"); // 
    }

    [HttpGet("/Admin/Prices/Edit")]
    public async Task<IActionResult> EditPrices(int? id)
    {
        if (id == null) return NotFound();
        var temp = await _context.Prices.FindAsync(id);
        if (temp == null) return NotFound();

        ViewData["DentistId"] = new SelectList(_context.Dentists, "ID", "Name");

        return View("~/Views/Admin/Prices/Edit.cshtml", temp); // 
    }

    [HttpGet("/Admin/Prices/Delete")]
    public async Task<IActionResult> DeletePrices(int? id)
    {
        if (id == null) return NotFound();
        var temp = await _context.Prices.FirstOrDefaultAsync(m => m.ID == id);
        if (temp == null) return NotFound();

        return View("~/Views/Admin/Prices/Delete.cshtml", temp); // 
    }

    //--------------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------Contact

    [Area("Admin")]
    [HttpGet("/Admin/Contact/AdminIndex")]
    public IActionResult Contact()
    {
        var dentist = _context.Contacts.ToList();
        return View("~/Views/Admin/Contact/AdminIndex.cshtml", dentist);
    }

    //--------------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------Prices

    [Area("Admin")]
    [HttpGet("/Admin/Patient/AdminIndex")]
    public IActionResult Patient()
    {
        var dentist = _context.Patients.ToList();
        return View("~/Views/Admin/Patient/AdminIndex.cshtml", dentist);
    }

    [HttpGet("/Admin/Patient/Create")]
    public IActionResult CreatePatient()
    {
        return View("~/Views/Admin/Patient/Create.cshtml"); // 
    }

    [HttpGet("/Admin/Patient/Edit")]
    public async Task<IActionResult> EditPatient(int? id)
    {
        if (id == null) return NotFound();
        var temp = await _context.Prices.FindAsync(id);
        if (temp == null) return NotFound();


        return View("~/Views/Admin/Patient/Edit.cshtml", temp); // 
    }

    [HttpGet("/Admin/Patient/Delete")]
    public async Task<IActionResult> DeletePatient(int? id)
    {
        if (id == null) return NotFound();
        var temp = await _context.Prices.FirstOrDefaultAsync(m => m.ID == id);
        if (temp == null) return NotFound();

        return View("~/Views/Admin/Patient/Delete.cshtml", temp); // 
    }
}












