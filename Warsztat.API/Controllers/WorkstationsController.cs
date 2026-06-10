using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Warsztat.API.Data;
using Warsztat.API.DTOs;
using Warsztat.API.Models;

namespace Warsztat.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Wymaga zalogowania
    public class WorkstationsController : ControllerBase
    {
        private readonly WorkshopDbContext _context;

        public WorkstationsController(WorkshopDbContext context)
        {
            _context = context;
        }

        // GET: api/workstations
        [HttpGet]
        public async Task<IActionResult> GetWorkstations()
        {
            var workstations = await _context.Workstations
                .Select(w => new WorkstationDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    Type = w.Type,
                    IsActive = w.IsActive
                })
                .ToListAsync();

            return Ok(workstations);
        }

        // POST: api/workstations (Tylko dla Szefa!)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateWorkstation(CreateWorkstationDto dto)
        {
            var workstation = new Workstation
            {
                Name = dto.Name,
                Type = dto.Type,
                IsActive = true
            };

            _context.Workstations.Add(workstation);
            await _context.SaveChangesAsync();

            return Ok(new WorkstationDto
            {
                Id = workstation.Id,
                Name = workstation.Name,
                Type = workstation.Type,
                IsActive = workstation.IsActive
            });
        }
    }
}