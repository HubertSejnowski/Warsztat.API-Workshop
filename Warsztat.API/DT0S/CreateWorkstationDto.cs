using System.ComponentModel.DataAnnotations;

namespace Warsztat.API.DTOs
{
    public class CreateWorkstationDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Type { get; set; } = string.Empty;
    }
}