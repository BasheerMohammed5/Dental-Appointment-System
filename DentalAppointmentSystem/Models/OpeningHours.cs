using System.ComponentModel.DataAnnotations;

namespace DentalAppointmentSystem.Models
{
    public class OpeningHours
    {
        public int ID { get; set; }


        [Required(ErrorMessage = "Day is required")]
        public string Day { get; set; }


        [Required(ErrorMessage = "From time is required")]
        [DataType(DataType.Time)]
        public TimeSpan From { get; set; }

        [Required(ErrorMessage = "To time is required")]
        [DataType(DataType.Time)]
        public TimeSpan To { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan? From2 { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan? To2 { get; set; }

        [Required(ErrorMessage = "Dentist must be selected")]
        public int DentistId { get; set; }
        public Dentist Dentist { get; set; }
    }
}
