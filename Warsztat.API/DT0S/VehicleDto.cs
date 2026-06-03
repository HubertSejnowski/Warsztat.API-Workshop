namespace Warsztat.API.DT0S
{
    public class VehicleDto
    {
        public int Id { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int ProductionYear { get; set; }
        public string VIN { get; set; } = string.Empty;
        public string? EngineCode { get; set; }
        public int CustomerId { get; set; }
    }
}
