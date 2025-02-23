using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkiServiceAPI.Models;
using MongoDB.Driver;
using SkiServiceAPI.Services;

namespace SkiServiceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IMongoCollection<Account> _accounts;

        public AccountController(ILogger<AccountController> logger, MongoDbService mongoDbService)
        {
            _logger = logger;

            _accounts = mongoDbService.Database.GetCollection<Account>("account");
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] Account account)
        {
            // Prüfe, ob der Benutzername bereits existiert
            if (_accounts.Find(a => a.Benutzername == account.Benutzername).Any())
            {
                return BadRequest(new { message = "Benutzername existiert bereits" });
            }

            // Prüfe, ob die Email bereits existiert
            if (_accounts.Find(a => a.Email == account.Email).Any())
            {
                return BadRequest(new { message = "Email existiert bereits" });
            }

            // Passwort prüfen und hashen
            if (!string.IsNullOrEmpty(account.PasswortHash))
            {
                account.PasswortHash = BCrypt.Net.BCrypt.HashPassword(account.PasswortHash);
            }
            else
            {
                return BadRequest(new { message = "Passwort darf nicht leer sein." });
            }

            if (ModelState.IsValid)
            {
                // Füge den neuen Account in die MongoDB Collection ein
                _accounts.InsertOne(account);
                return Ok(new { message = "Account erfolgreich erstellt" });
            }

            return BadRequest(new { message = "Ungültige Daten!" });
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin,Mitarbeiter")]
        public IActionResult Delete(string id)
        {
            // Lösche den Account anhand der ID
            var result = _accounts.DeleteOne(a => a.AccountID == id);
            if (result.DeletedCount == 0)
            {
                return NotFound(new { message = "Account nicht gefunden." });
            }
            return Ok(new { message = "Account erfolgreich gelöscht." });
        }
    }
}
