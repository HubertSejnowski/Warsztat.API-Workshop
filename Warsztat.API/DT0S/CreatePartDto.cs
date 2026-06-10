using System.ComponentModel.DataAnnotations;

namespace Warsztat.API.DTOs
{
    public class CreatePartDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string PartNumber { get; set; } = string.Empty;

        [Range(0.01, 100000)]
        public decimal UnitPrice { get; set; }

        public bool IsOEM { get; set; }
    }
}