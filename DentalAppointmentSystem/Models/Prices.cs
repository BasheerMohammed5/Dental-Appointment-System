using Microsoft.AspNetCore.Hosting.Server;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalAppointmentSystem.Models
{
    public class Prices
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "")]
        public int ServerId { get; set; }
        public Service Server { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Column(TypeName = "decimal(18,2)")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
    }
}
