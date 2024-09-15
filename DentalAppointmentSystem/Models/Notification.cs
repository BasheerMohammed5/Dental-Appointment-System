using System.ComponentModel.DataAnnotations;

namespace DentalAppointmentSystem.Models
{
    public class Notification
    {
        public int ID { get; set; }

        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; }

        [Required]
        public string NotificationType { get; set; } // "Reminder", "Cancellation", etc.

        [Required]
        public string Message { get; set; }

        [Required]
        public DateTime ScheduledFor { get; set; }

        public bool IsSent { get; set; }

        public DateTime? SentAt { get; set; }

        public int? PatientDetailsId { get; set; }
        public Patient PatientDetails { get; set; }
    }
}
