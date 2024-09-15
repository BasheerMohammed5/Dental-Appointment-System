using Microsoft.AspNetCore.Hosting.Server;
using System.ComponentModel.DataAnnotations;

namespace DentalAppointmentSystem.Models
{
    public class Appointment
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Dentist must be selected")]
        public int DentistId { get; set; }
        public Dentist Dentist { get; set; }

        [Required(ErrorMessage = "Service must be selected")]
        public int ServerId { get; set; }
        public Service Server { get; set; }

        [Required]
        [Display(Name = "Patient Name")]
        public string PatientName { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string Phone { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of the appointment")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Time is required")]
        [DataType(DataType.Time)]
        [Display(Name = "Appointment time")]
        public TimeSpan Time { get; set; }

        [Required]
        [StringLength(8)]
        public string BookingCode { get; set; } // سيتم إنشاؤه تلقائيًا

        [Range(15, 240, ErrorMessage = "Duration must be between 15 and 240 minutes")]
        [Display(Name = "Duration (minutes)")]
        public int? DurationInMinutes { get; set; }

        [Required(ErrorMessage = "Reason for visit is required")]
        [Display(Name = "Reason for visit")]
        public string ReasonForVisit { get; set; }

        [Display(Name = "Created at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public AppointmentStatus Status { get; set; } = AppointmentStatus.New;
    }

    public enum AppointmentStatus
    {
        New,
        Scheduled,
        Confirmed,
        Completed,
        Cancelled,
        Rescheduled,
        CompletedDone,
        NoShow,
        Transferred,
        Pending,
        Reviewed,
        UnderReview
    }
}
