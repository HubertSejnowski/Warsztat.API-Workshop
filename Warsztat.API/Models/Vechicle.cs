using System.ComponentModel.DataAnnotations;

namespace Warsztat.API.Models
{
    public class Vehicle
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Brand { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Model { get; set; } = string.Empty;

        public int ProductionYear { get; set; }

        [Required]
        [StringLength(17, MinimumLength = 17)]
        public string VIN { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? EngineCode { get; set; } // Przydatne przy dobieraniu części i diagnostyce

        // Relacja do Klienta
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        // Relacja: Jeden pojazd ma historię wielu zleceń
        public ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
    }
}