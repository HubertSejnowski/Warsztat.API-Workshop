

namespace Warsztat.API.Models
{
    public class UsedPart
    {
        public int Id { get; set; }

        public int WorkOrderId { get; set; }
        public WorkOrder? WorkOrder { get; set; }

        public int PartId { get; set; }
        public Part? Part { get; set; }

        public int Quantity { get; set; } // Ilość zużytych sztuk/litrów
    }
}