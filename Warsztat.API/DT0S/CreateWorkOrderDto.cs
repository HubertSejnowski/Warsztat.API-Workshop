using System.ComponentModel.DataAnnotations;

namespace Warsztat.API.DT0S
{
    public class CreateWorkOrderDto
    {
        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime ScheduledDate { get; set; }

        [Required]
        public int VehicleId { get; set; }

        public int? WorkstationId { get; set; }
    }
}
