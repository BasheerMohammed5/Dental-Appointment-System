using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DentalAppointmentSystem.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;

public class NotificationController : Controller
{
    private readonly ApplicationDbContext _context;

    public NotificationController(ApplicationDbContext context)
    {
        _context = context;
    }

    // عرض جميع التنبيهات
    public async Task<IActionResult> Index()
    {
        var notifications = await _context.Notifications
            .Include(n => n.Appointment)
            //.Include(n => n.Patient)
            .ToListAsync();
        return View(notifications);
    }

    // البحث عن التنبيهات
    [HttpPost]
    public async Task<IActionResult> Search(string searchTerm)
    {
        var notifications = await _context.Notifications
            .Include(n => n.Appointment)
            .Where(n => n.Message.Contains(searchTerm) ||
                        n.NotificationType.Contains(searchTerm))
            .ToListAsync();
        return View("Index", notifications);
    }

    // حفظ تنبيه جديد
    [HttpPost]
    public async Task<IActionResult> Create(Notification notification)
    {
        if (ModelState.IsValid)
        {
            _context.Add(notification);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(notification);
    }

    // تعديل تنبيه
    [HttpPost]
    public async Task<IActionResult> Edit(int id, Notification notification)
    {
        if (id != notification.ID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            _context.Update(notification);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(notification);
    }

    // حذف تنبيه
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var notification = await _context.Notifications.FindAsync(id);
        if (notification != null)
        {
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    // دالة فحص الحجوزات وإرسال تذكيرات قبل يوم واحد من تاريخ الحجز
    public async Task<IActionResult> CheckAndSendReminders()
    {
        var tomorrow = DateTime.Now.Date.AddDays(1);

        var appointments = await _context.Appointments
            .Where(a => a.Date.Date == tomorrow)
            .ToListAsync();

        foreach (var appointment in appointments)
        {
            // إنشاء الإشعار
            var notification = new Notification
            {
                AppointmentId = appointment.ID,
                NotificationType = "Reminder",
                Message = $"Reminder: You have an appointment on {appointment.Date.ToShortDateString()} at {appointment.Time}.",
                ScheduledFor = DateTime.Now,
                //PatientsId = null
            };

            // حفظ الإشعار
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // إرسال رسالة تذكيرية عبر البريد الإلكتروني أو الهاتف
            SendNotification(notification, appointment.PatientName, appointment.Phone, appointment.Email);
        }

        // فحص PatientDetails لإرسال تنبيهات العودة
        var patientsWithReturnDate = await _context.PatientDetails
            .Where(p => p.ReturnDate.HasValue && p.ReturnDate.Value.Date == tomorrow)
            .ToListAsync();

        foreach (var patientDetail in patientsWithReturnDate)
        {
            // إنشاء رسالة التذكير بالعودة
            var notification = new Notification
            {
                AppointmentId = 0, // ليس هناك حجز
                NotificationType = "Return Reminder",
                Message = $"Reminder: You have a follow-up return scheduled on {patientDetail.ReturnDate.Value.ToShortDateString()}.",
                ScheduledFor = DateTime.Now,
                //PatientsId = patientDetail.PatientID
            };

            // حفظ الإشعار
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // إرسال رسالة تذكيرية عبر البريد الإلكتروني أو الهاتف
            var patient = await _context.Patients.FindAsync(patientDetail.PatientID);
            SendNotification(notification, patient.Name, patient.Phone, patient.Email);
        }

        return Ok("Reminders sent successfully.");
    }

    // دالة لإرسال التنبيهات عبر الإيميل أو الهاتف
    public void SendNotification(Notification notification, string patientName, string phone, string email)
    {
        // إرسال عبر البريد الإلكتروني
        if (!string.IsNullOrEmpty(email))
        {
            SendEmail(email, "Appointment Reminder", notification.Message);
        }

        // إرسال عبر الهاتف (هنا تحتاج لمزود خدمة SMS)
        if (!string.IsNullOrEmpty(phone))
        {
            SendSMS(phone, notification.Message);
        }

        // تحديث حالة الإشعار إلى "تم الإرسال"
        notification.IsSent = true;
        notification.SentAt = DateTime.Now;
        _context.Update(notification);
        _context.SaveChanges();
    }

    // دوال إرسال البريد الإلكتروني والرسائل النصية
    private void SendEmail(string email, string subject, string message)
    {
        // استخدام SMTP 
        var smtpClient = new SmtpClient("smtp.your-email-provider.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("your-email@example.com", "your-email-password"),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress("your-email@example.com"),
            Subject = subject,
            Body = message,
            IsBodyHtml = true,
        };
        mailMessage.To.Add(email);

        smtpClient.Send(mailMessage);


        //استخدام SendGrid
        //var client = new SendGridClient("your_sendgrid_api_key");
        //var from = new EmailAddress("youremail@example.com", "Your Name");
        //var to = new EmailAddress(email);
        //var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);
        //var response = client.SendEmailAsync(msg).Result;
    }

    private void SendSMS(string phoneNumber, string message)
    {
        // استخدام Twilio
        //TwilioClient.Init("your_twilio_account_sid", "your_twilio_auth_token");

        //var messageOptions = new CreateMessageOptions(
        //    new PhoneNumber(phoneNumber))
        //{
        //    From = new PhoneNumber("your_twilio_phone_number"),
        //    Body = message
        //};

        //var msg = MessageResource.Create(messageOptions);
    }
}
