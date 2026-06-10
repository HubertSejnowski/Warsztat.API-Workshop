namespace Warsztat.API.DT0S
{
    public class OrderSummaryDto
    {
        public int OrderId { get; set; }
        public string CustomerFullName { get; set; } = string.Empty;
        public string VehicleInfo { get; set; } = string.Empty; 
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? CompletionDate { get; set; }

        // part list
        public List<UsedPartDto> Parts { get; set; } = new List<UsedPartDto>();

        // Cost
        public decimal TotalCost { get; set; }
    }
}
