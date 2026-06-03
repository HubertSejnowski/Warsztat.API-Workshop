using System.ComponentModel.DataAnnotations;

namespace Warsztat.API.Models
{
    public class Workstation
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty; // np. "Kanał główny", "Stanowisko diagnostyczne"

        [Required]
        [MaxLength(30)]
        public string Type { get; set; } = string.Empty; // np. Pit, Lift, Diagnostic

        public bool IsActive { get; set; } = true;
    }
}