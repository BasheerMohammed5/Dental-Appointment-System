using System.ComponentModel.DataAnnotations;

namespace DentalAppointmentSystem.Models
{
    public class Dentist
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Dentist name is required")]
        [StringLength(100, ErrorMessage = "Name must be between 2 and 100 characters", MinimumLength = 2)]
        [Display(Name = "Dentist Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Specialization is required")]
        [Display(Name = "Specialization")]
        public string Specialization { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        public string? Image { get; set; }

        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string? Email { get; set; }

        public string? X { get; set; }
        public string? Facebook { get; set; }
        public string? LinkedIn { get; set; }
        public string? Instagram { get; set; }

        public int? ServerId { get; set; }
        public Service Server { get; set; }


        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<OpeningHours> WorkingHours { get; set; }
    }
}
