using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Warsztat.API.Data;
using Warsztat.API.DT0S;
using Warsztat.API.Models;

namespace Warsztat.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkOrdersController : ControllerBase
    {
        private readonly WorkshopDbContext _context;

        public WorkOrdersController(WorkshopDbContext context)
        {
            _context = context;
        }

        // GET: api/workorders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkOrderDto>>> GetWorkOrders()
        {
            var orders = await _context.WorkOrders
                .Include(wo => wo.UsedParts)
                .ThenInclude(up => up.Part)
                .Select(wo => new WorkOrderDto
                {
                    Id = wo.Id,
                    Description = wo.Description,
                    DiagnosticNotes = wo.DiagnosticNotes,
                    Status = wo.Status.ToString(),
                    ScheduledDate = wo.ScheduledDate,
                    CompletionDate = wo.CompletionDate,
                    VehicleId = wo.VehicleId,
                    WorkstationId = wo.WorkstationId,
                    UsedParts = wo.UsedParts.Select(up => new UsedPartDto
                    {
                        PartId = up.PartId,
                        // Zmiana z up.Part?.Name na klasyczny warunek ternary (nie wyrzuca błędu CS8072)
                        PartName = up.Part != null ? up.Part.Name : "Nieznana część",
                        PartNumber = up.Part != null ? up.Part.PartNumber : string.Empty,
                        IsOEM = up.Part != null ? up.Part.IsOEM : false,
                        Quantity = up.Quantity,
                        TotalPrice = up.Quantity * (up.Part != null ? up.Part.UnitPrice : 0)
                    }).ToList()
                })
                .ToListAsync();

            return Ok(orders);
        }

        // POST: api/workorders
        [HttpPost]
        [Authorize(Roles = "Admin,Reception")]
        public async Task<ActionResult<WorkOrderDto>> CreateWorkOrder(CreateWorkOrderDto createDto)
        {
            // 1. Sprawdzamy, czy auto istnieje
            var vehicleExists = await _context.Vehicles.AnyAsync(v => v.Id == createDto.VehicleId);
            if (!vehicleExists)
            {
                return BadRequest("Podany pojazd nie istnieje w bazie danych.");
            }

            // 2. Obsługa stanowiska (wykona się TYLKO, gdy workstationId nie jest null)
            if (createDto.WorkstationId.HasValue)
            {
                var workstationExists = await _context.Workstations.AnyAsync(w => w.Id == createDto.WorkstationId.Value);
                if (!workstationExists)
                {
                    return BadRequest("Podane stanowisko warsztatowe nie istnieje.");
                }

                // WALIDACJA HARMONOGRAMU (Teraz jest bezpiecznie schowana wewnątrz if'a)
                var isOccupied = await _context.WorkOrders.AnyAsync(wo =>
                    wo.WorkstationId == createDto.WorkstationId.Value &&
                    (wo.Status == OrderStatus.Planned || wo.Status == OrderStatus.InProgress) &&
                    wo.ScheduledDate.Date == createDto.ScheduledDate.Date &&
                    wo.ScheduledDate.Hour == createDto.ScheduledDate.Hour);

                if (isOccupied)
                {
                    return BadRequest("Błąd harmonogramu: Wybrane stanowisko jest już zajęte w tym terminie!");
                }
            }

            // 3. Zapisujemy zlecenie do bazy
            var order = new WorkOrder
            {
                Description = createDto.Description,
                ScheduledDate = createDto.ScheduledDate,
                VehicleId = createDto.VehicleId,
                WorkstationId = createDto.WorkstationId, // Tu zapisze się int albo null
                Status = OrderStatus.Planned
            };

            _context.WorkOrders.Add(order);
            await _context.SaveChangesAsync();

            return Ok($"Zlecenie nr {order.Id} zostało pomyślnie zapisane w harmonogramie.");
        }
        

        // POST: api/workorders/{id}/parts
        [HttpPost("{id}/parts")]
        public async Task<IActionResult> AddPartToOrder(int id, AddPartToOrderDto partDto)
        {
            var order = await _context.WorkOrders.FindAsync(id);
            if (order == null)
            {
                return NotFound("Nie znaleziono takiego zlecenia naprawy.");
            }

            var part = await _context.Parts.FindAsync(partDto.PartId);
            if (part == null)
            {
                return NotFound("Wybrana część nie istnieje w magazynie.");
            }

            // Tworzymy powiązanie w tabeli pośredniczącej
            var usedPart = new UsedPart
            {
                WorkOrderId = id,
                PartId = partDto.PartId,
                Quantity = partDto.Quantity
            };

            _context.UsedParts.Add(usedPart);
            await _context.SaveChangesAsync();

            return Ok($"Pomyślnie dodano część do zlecenia. Do naliczenia: {partDto.Quantity}x {part.Name} (OEM: {part.IsOEM})");
        }

        [HttpGet("{id}/summary")]
        [Authorize(Roles = "Admin,Reception")]
        public async Task<ActionResult<OrderSummaryDto>> GetOrderSummary(int id)
        {
            var order = await _context.WorkOrders
                .Include(wo => wo.Vehicle)
                    .ThenInclude(v => v.Customer)
                .Include(wo => wo.UsedParts)
                    .ThenInclude(up => up.Part)   // Wyciągamy szczegóły części
                .FirstOrDefaultAsync(wo => wo.Id == id);

            if (order == null)
            {
                return NotFound("Nie znaleziono zlecenia.");
            }

            // Obliczamy całkowity koszt użytych części
            decimal totalPartsCost = order.UsedParts.Sum(up => up.Quantity * (up.Part != null ? up.Part.UnitPrice : 0));

            // Dodajemy stałą opłatę za robociznę (w przyszłości można to przenieść do bazy)
            decimal laborCost = 150.00m;
            decimal finalCost = totalPartsCost + laborCost;

            // Budujemy ładne podsumowanie
            var summary = new OrderSummaryDto
            {
                OrderId = order.Id,
                CustomerFullName = order.Vehicle?.Customer != null
                    ? $"{order.Vehicle.Customer.FirstName} {order.Vehicle.Customer.LastName} (Tel: {order.Vehicle.Customer.PhoneNumber})"
                    : "Klient nieznany",
                VehicleInfo = $"{order.Vehicle?.Brand} {order.Vehicle?.Model} (VIN: {order.Vehicle?.VIN})",
                Description = order.Description,
                Status = order.Status.ToString(),
                CompletionDate = order.CompletionDate,
                Parts = order.UsedParts.Select(up => new UsedPartDto
                {
                    PartId = up.PartId,
                    PartName = up.Part != null ? up.Part.Name : "Nieznana część",
                    PartNumber = up.Part != null ? up.Part.PartNumber : string.Empty,
                    IsOEM = up.Part != null ? up.Part.IsOEM : false,
                    Quantity = up.Quantity,
                    TotalPrice = up.Quantity * (up.Part != null ? up.Part.UnitPrice : 0)
                }).ToList(),
                TotalCost = finalCost
            };

            return Ok(summary);


        }




    }
}
