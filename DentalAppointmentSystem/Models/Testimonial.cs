using System.ComponentModel.DataAnnotations;

namespace DentalAppointmentSystem.Models
{
    public class Testimonial
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name must be between 2 and 100 characters", MinimumLength = 2)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Text is required")]
        [StringLength(500, ErrorMessage = "Text must be between 10 and 500 characters", MinimumLength = 10)]
        public string Text { get; set; }

        public string? Image { get; set; }

        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }
    }
}
