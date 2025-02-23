using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkiServiceAPI.Models;

namespace SkiServiceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly SkiServiceContext _context;

        public AccountController(ILogger<AccountController> logger, SkiServiceContext context)
        {
            _logger = logger;
            _context = context;
        }





        [HttpPost("create")]
        public IActionResult Create([FromBody] Account account)
        {
            if (_context.Accounts.Any(a => a.Benutzername == account.Benutzername))
            {
                return BadRequest(new { message = "Benutzername existiert bereits" });
            }

            if (_context.Accounts.Any(a => a.Email == account.Email))
            {
                return BadRequest(new { message = "Email existiert bereits" });
            }


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
                _context.Accounts.Add(account);
                _context.SaveChanges();
                return Ok(new { message = "Account erfolgreich erstellt" });
            }

            return BadRequest(new { message = "Ungültige Daten!" });
        }



// PUT: api/account/update/{id}
        [HttpPut("update/{id}")]
        [Authorize(Roles = "Admin,Mitarbeiter")]
        public IActionResult Update(int id, [FromBody] Account updatedAccount)
        {
            // Account suchen
            var existingAccount = _context.Accounts.FirstOrDefault(a => a.AccountID == id);
            if (existingAccount == null)
            {
                return NotFound(new { message = "Account nicht gefunden." });
            }

            // Benutzername überprüfen
            if (_context.Accounts.Any(a => a.Benutzername == updatedAccount.Benutzername && a.AccountID != id))
            {
                return BadRequest(new { message = "Benutzername existiert bereits." });
            }

            // Email überprüfen
            if (_context.Accounts.Any(a => a.Email == updatedAccount.Email && a.AccountID != id))
            {
                return BadRequest(new { message = "Email existiert bereits." });
            }

            // Account aktualisieren
            existingAccount.Benutzername = updatedAccount.Benutzername;
            existingAccount.Email = updatedAccount.Email;

            if (!string.IsNullOrEmpty(updatedAccount.PasswortHash))
            {
                existingAccount.PasswortHash = BCrypt.Net.BCrypt.HashPassword(updatedAccount.PasswortHash);
            }

            existingAccount.Telefon = updatedAccount.Telefon;

            _context.SaveChanges();
            return Ok(new { message = "Account erfolgreich aktualisiert." });
        }



        // DELETE: api/account/delete/{id}
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin,Mitarbeiter")]
        public IActionResult Delete(int id)
        {
            // Account suchen
            var account = _context.Accounts.FirstOrDefault(a => a.AccountID == id);
            if (account == null)
            {
                return NotFound(new { message = "Account nicht gefunden." });
            }

            // Account löschen
            _context.Accounts.Remove(account);
            _context.SaveChanges();
            return Ok(new { message = "Account erfolgreich gelöscht." });
        }
    }
}
