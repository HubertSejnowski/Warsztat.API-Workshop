using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warsztat.API.Models
{
    public class Part
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string PartNumber { get; set; } = string.Empty; // Numer katalogowy

        public bool IsOEM { get; set; } // True = Oryginał, False = Zamiennik

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
    }
}