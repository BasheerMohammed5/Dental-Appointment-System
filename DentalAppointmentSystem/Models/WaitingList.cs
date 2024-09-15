using System.ComponentModel.DataAnnotations;

namespace DentalAppointmentSystem.Models
{
    public class WaitingList
    {
        public int ID { get; set; }

        [Required]
        public int ServerId { get; set; }
        public Service Server { get; set; }

        [Required]
        public int DentistId { get; set; }
        public Dentist Dentist { get; set; }

        [Required]
        public string PatientName { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        public string? EmailAddress { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime PreferredDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsNotified { get; set; } = false;
    }
}
