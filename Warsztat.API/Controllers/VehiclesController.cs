using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Warsztat.API.Data;
using Warsztat.API.DT0S;
using Warsztat.API.Models;

namespace Warsztat.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesController : ControllerBase
    {
        private readonly WorkshopDbContext _context;

        public VehiclesController(WorkshopDbContext context)
        {
            _context = context;
        }

        // GET: api/vehicles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VehicleDto>>> GetVehicles()
        {
            var vehicles = await _context.Vehicles
                .Select(v => new VehicleDto
                {
                    Id = v.Id,
                    Brand = v.Brand,
                    Model = v.Model,
                    ProductionYear = v.ProductionYear,
                    VIN = v.VIN,
                    EngineCode = v.EngineCode,
                    CustomerId = v.CustomerId
                })
                .ToListAsync();

            return Ok(vehicles);
        }

        // POST: api/vehicles
        [HttpPost]
        public async Task<ActionResult<VehicleDto>> CreateVehicle(CreateVehicleDto createDto)
        {
            // WALIDACJA: Sprawdzamy, czy klient (właściciel) istnieje w bazie danych
            var customerExists = await _context.Customers.AnyAsync(c => c.Id == createDto.CustomerId);
            if (!customerExists)
            {
                return BadRequest("Błąd: Nie można dodać pojazdu. Klient o podanym ID nie istnieje.");
            }

            // Mapowanie DTO na model bazodanowy
            var vehicle = new Vehicle
            {
                Brand = createDto.Brand,
                Model = createDto.Model,
                ProductionYear = createDto.ProductionYear,
                VIN = createDto.VIN,
                EngineCode = createDto.EngineCode,
                CustomerId = createDto.CustomerId
            };

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            // Przygotowanie DTO wynikowego
            var resultDto = new VehicleDto
            {
                Id = vehicle.Id,
                Brand = vehicle.Brand,
                Model = vehicle.Model,
                ProductionYear = vehicle.ProductionYear,
                VIN = vehicle.VIN,
                EngineCode = vehicle.EngineCode,
                CustomerId = vehicle.CustomerId
            };

            return CreatedAtAction(nameof(GetVehicles), new { id = vehicle.Id }, resultDto);
        }
    }
}