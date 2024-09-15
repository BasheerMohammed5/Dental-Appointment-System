using System.ComponentModel.DataAnnotations;

namespace DentalAppointmentSystem.Models
{
    public class Service
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Server Name")]
        public string Name { get; set; }

        public string? Description { get; set; }

        public string? Images { get; set; }

        [Range(15, 240, ErrorMessage = "Duration must be between 15 and 240 minutes")]
        [Display(Name = "Duration (minutes)")]
        public int? DurationInMinutes { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
    }
}
