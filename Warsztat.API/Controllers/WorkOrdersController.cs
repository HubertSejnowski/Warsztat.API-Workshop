using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<WorkOrderDto>> CreateWorkOrder(CreateWorkOrderDto createDto)
        {
            // Walidacja: Czy pojazd istnieje w systemie
            var vehicleExists = await _context.Vehicles.AnyAsync(v => v.Id == createDto.VehicleId);
            if (!vehicleExists)
            {
                return BadRequest("Podany pojazd nie istnieje w bazie danych.");
            }

            // Jeśli wybrano stanowisko, sprawdź czy istnieje
            if (createDto.WorkstationId.HasValue)
            {
                var workstationExists = await _context.Workstations.AnyAsync(w => w.Id == createDto.WorkstationId.Value);
                if (!workstationExists)
                {
                    return BadRequest("Podane stanowisko warsztatowe nie istnieje.");
                }
            }

            var order = new WorkOrder
            {
                Description = createDto.Description,
                ScheduledDate = createDto.ScheduledDate,
                VehicleId = createDto.VehicleId,
                WorkstationId = createDto.WorkstationId,
                Status = OrderStatus.Planned
            };

            _context.WorkOrders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetWorkOrders), new { id = order.Id }, new WorkOrderDto
            {
                Id = order.Id,
                Description = order.Description,
                Status = order.Status.ToString(),
                ScheduledDate = order.ScheduledDate,
                VehicleId = order.VehicleId,
                WorkstationId = order.WorkstationId
            });
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
    }
}
