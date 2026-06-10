using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Warsztat.API.Data;
using Warsztat.API.DTOs;
using Warsztat.API.Models;

namespace Warsztat.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PartsController : ControllerBase
    {
        private readonly WorkshopDbContext _context;

        public PartsController(WorkshopDbContext context)
        {
            _context = context;
        }

        // POST: api/parts
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreatePart(CreatePartDto dto)
        {
            var part = new Part
            {
                Name = dto.Name,
                PartNumber = dto.PartNumber,
                UnitPrice = dto.UnitPrice,
                IsOEM = dto.IsOEM
            };

            _context.Parts.Add(part);
            await _context.SaveChangesAsync();

            return Ok(new { part.Id, part.Name, part.UnitPrice });
        }
    }
}