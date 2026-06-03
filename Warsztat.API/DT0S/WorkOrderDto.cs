namespace Warsztat.API.DT0S
{
    public class WorkOrderDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? DiagnosticNotes { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime ScheduledDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public int VehicleId { get; set; }
        public int? WorkstationId { get; set; }
        public List<UsedPartDto> UsedParts { get; set; } = new List<UsedPartDto>();
    }
    public class UsedPartDto
    {
        public int PartId { get; set; }
        public string PartName { get; set; } = string.Empty;
        public string PartNumber { get; set; } = string.Empty;
        public bool IsOEM { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }

}
