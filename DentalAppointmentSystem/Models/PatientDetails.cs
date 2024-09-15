using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace DentalAppointmentSystem.Models
{
    public class PatientDetails
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Patient must be selected")]
        public int PatientID { get; set; }
        public Patient Patient { get; set; }

        public string? RegisterNumber { get; set; }

        public int? ServerId { get; set; }
        public Service Server { get; set; }

        public string? Address { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ReturnDate { get; set; }

        public string? Notes { get; set; }

        public string? files { get; set; }

        public string? BloodType { get; set; }

        public string? Allergies { get; set; }
    }
}
