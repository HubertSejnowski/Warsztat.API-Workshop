using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Warsztat.API.Data;
using Warsztat.API.DT0S;
using Warsztat.API.Models;



namespace Warsztat.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly WorkshopDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(WorkshopDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto request)
        {
            // 1. Szukamy użytkownika w bazie
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

            // 2. Jeśli nie ma usera lub hasło się nie zgadza (używamy BCrypt do weryfikacji)
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized("Nieprawidłowa nazwa użytkownika lub hasło.");
            }

            // 3. Tworzymy zawartość tokena (Claims)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            // 4. Pobieramy klucz z appsettings.json
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            // 5. Budujemy token (ważny np. przez 1 dzień)
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            // 6. Zwracamy token do użytkownika
            return Ok(new { Token = jwt });
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(LoginDto request)
        {
            // 1. Sprawdzamy, czy użytkownik o takiej nazwie już istnieje
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return BadRequest("Użytkownik o takiej nazwie już istnieje w bazie.");
            }

            // 2. Tworzymy nowego użytkownika i haszujemy hasło na żywo!
            var user = new User
            {
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = "Admin" // Na start dajemy każdemu uprawnienia administratora
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Użytkownik został pomyślnie zarejestrowany! Możesz się teraz zalogować.");
        }
    }
}