using System.ComponentModel.DataAnnotations;

namespace DentalAppointmentSystem.Models
{
    public class Contact
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name must be between 2 and 100 characters", MinimumLength = 2)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Subject is required")]
        [StringLength(200, ErrorMessage = "Subject must be between 2 and 200 characters", MinimumLength = 2)]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Message is required")]
        [StringLength(1000, ErrorMessage = "Message must be between 10 and 1000 characters", MinimumLength = 10)]
        public string Message { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        public DateTime SentAt { get; set; } = DateTime.Now;
    }
}
