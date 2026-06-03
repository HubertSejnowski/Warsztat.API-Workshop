using System.ComponentModel.DataAnnotations;

namespace Warsztat.API.Models
{
    public enum OrderStatus
    {
        Planned,     // Zaplanowane
        InProgress,  // W trakcie naprawy
        Completed,   // Zakończone
        Cancelled    // Odwołane
    }

    public class WorkOrder
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty; // Opis usterki / zadania

        public string? DiagnosticNotes { get; set; } // Logi z kodowania, błędy diagnostyczne

        public OrderStatus Status { get; set; } = OrderStatus.Planned;

        public DateTime ScheduledDate { get; set; } // Kiedy wizyta ma się odbyć
        public DateTime? CompletionDate { get; set; }

        // Relacja do Pojazdu
        public int VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }

        // Relacja do Stanowiska/Kanału (może być przypisane lub nie na początku)
        public int? WorkstationId { get; set; }
        public Workstation? Workstation { get; set; }

        // Relacja wiele-do-wielu z częściami (poprzez tabelę pośredniczącą)
        public ICollection<UsedPart> UsedParts { get; set; } = new List<UsedPart>();
    }
}