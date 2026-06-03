using System.ComponentModel.DataAnnotations;

namespace Warsztat.API.DT0S
{
    public class AddPartToOrderDto
    {
        [Required]
        public int PartId { get; set; }

        [Required]
        [Range(1, 100)]
        public int Quantity { get; set; }
    }
}
