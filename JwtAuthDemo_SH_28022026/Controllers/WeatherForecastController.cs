using JwtAuthDemo.Models;
using JwtAuthDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace JwtAuthDemo_SH_28022026.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild",
            "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        // AVOIN ENDPOINT
        [HttpGet("open")]
        public IEnumerable<WeatherForecast> GetOpen()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            });
        }

        // LOGIN
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserCredentials credentials)
        {
            var tokenService = new TokenService();

            // Tavallinen k‰ytt‰j‰
            if (credentials.Username == "testuser" && credentials.Password == "testpassword")
            {
                var token = tokenService.GenerateToken(credentials.Username, false);
                return Ok(new { Token = token });
            }

            // Admin-k‰ytt‰j‰
            if (credentials.Username == "admin" && credentials.Password == "adminpassword")
            {
                var token = tokenService.GenerateToken(credentials.Username, true);
                return Ok(new { Token = token });
            }

            // V‰‰r‰t tunnukset
            return Unauthorized("K‰ytt‰j‰tunnus tai salasana on v‰‰rin.");
        }

        // SUOJATTU ENDPOINT
        [Authorize]
        [HttpGet("secure")]
        public IActionResult GetSecure()
        {
            return Ok(new { message = "T‰m‰ on suojattu endpoint. Token toimii!" });
        }

        // ADMIN-ENDPOINT (Authorize Roles)
        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public IActionResult GetAdmin()
        {
            return Ok("Vain Admin-roolin k‰ytt‰j‰t n‰kev‰t t‰m‰n.");
        }

        // ADMIN-POLICY ENDPOINT (RequireAdminRole)
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("admin-only")]
        public IActionResult GetAdminOnly()
        {
            return Ok("T‰m‰ endpoint n‰kyy vain Admin-roolin k‰ytt‰jille (policy).");
        }
    }
}