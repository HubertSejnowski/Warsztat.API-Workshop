using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Warsztat.API.Data;
using Warsztat.API.DT0s;
using Warsztat.API.DT0S;
using Warsztat.API.Models;


namespace Warsztat.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomersController : ControllerBase
    {

        private readonly WorkshopDbContext _context;

        public CustomersController(WorkshopDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
        {
            var customers = await _context.Customers
                .Select(c => new CustomerDto
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    PhoneNumber = c.PhoneNumber,
                    Email = c.Email
                })
                .ToListAsync();
            return Ok(customers);

        }

        // POST: api/customers
        [HttpPost]
        public async Task<ActionResult<CustomerDto>> CreateCustomer(CreateCustomerDto createDto)
        {
            // Mapujemy dane z formularza/żądania na obiekt bazy danych
            var customer = new Customer
            {
                FirstName = createDto.FirstName,
                LastName = createDto.LastName,
                PhoneNumber = createDto.PhoneNumber,
                Email = createDto.Email
            };

            // Zapisujemy w bazie danych
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Przygotowujemy obiekt zwrotny z nadanym już numerem Id
            var resultDto = new CustomerDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                PhoneNumber = customer.PhoneNumber,
                Email = customer.Email
            };

            // Zwracamy status 201 Created oraz stworzony obiekt
            return CreatedAtAction(nameof(GetCustomers), new { id = customer.Id }, resultDto);
        }
    }
}
