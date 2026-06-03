using System.ComponentModel.DataAnnotations;

namespace Warsztat.API.DT0S
{
    public class CreateVehicleDto
    {
        [Required]
        [MaxLength(50)]
        public string Brand { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Model { get; set; } = string.Empty;

        [Range(1900, 2100)]
        public int ProductionYear { get; set; }

        [Required]
        [StringLength(17, MinimumLength = 17)]
        public string VIN { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? EngineCode { get; set; } // Opcjonalne, ale ułatwia zaawansowaną diagnostykę

        [Required]
        public int CustomerId { get; set; } // ID właściciela samochodu
    }
}
