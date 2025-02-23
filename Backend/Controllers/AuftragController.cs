using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkiServiceAPI.DTOs;
using SkiServiceAPI.Models;

namespace SkiServiceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuftragController : ControllerBase
    {
        private readonly ILogger<AuftragController> _logger;
        private readonly SkiServiceContext _context;

        public AuftragController(ILogger<AuftragController> logger, SkiServiceContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] Auftrag auftrag)
        {
            if (auftrag == null)
            {
                return BadRequest(new { message = "Ungültige Daten!" });
            }

            // Kunde überprüfen
            var kunde = _context.Accounts.FirstOrDefault(a => a.AccountID == auftrag.KundeID);
            if (kunde == null)
            {
                return BadRequest(new { message = "Kunde nicht gefunden!" });
            }

            // Validierung prüfen
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage); // Debugging
                }
                return BadRequest(ModelState);
            }

            _context.Aufträge.Add(auftrag);
            _context.SaveChanges();
            return Ok(new { message = "Auftrag erfolgreich erstellt!", auftrag });
        }


        // PUT: api/auftrag/update/{id}
        [HttpPut("update/{id}")]
        [Authorize(Roles = "Admin,Mitarbeiter")]
        public IActionResult Update(int id, [FromBody] Auftrag updatedAuftrag)
        {
            var existingAuftrag = _context.Aufträge.FirstOrDefault(a => a.AuftragID == id);
            if (existingAuftrag == null)
            {
                return NotFound(new { message = "Auftrag nicht gefunden." });
            }

            if (ModelState.IsValid)
            {
                existingAuftrag.Dienstleistung = updatedAuftrag.Dienstleistung;
                existingAuftrag.Priorität = updatedAuftrag.Priorität;
                existingAuftrag.Status = updatedAuftrag.Status;

                _context.SaveChanges();
                return Ok(new { message = "Auftrag erfolgreich aktualisiert.", updatedAuftrag });
            }

            return BadRequest(new { message = "Ungültige Daten!" });
        }

        // DELETE: api/auftrag/delete/{id}
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin,Mitarbeiter")]
        public IActionResult Delete(int id)
        {
            var auftrag = _context.Aufträge.FirstOrDefault(a => a.AuftragID == id);
            if (auftrag == null)
            {
                return NotFound(new { message = "Auftrag nicht gefunden." });
            }

            _context.Aufträge.Remove(auftrag);
            _context.SaveChanges();
            return Ok(new { message = "Auftrag erfolgreich gelöscht." });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var auftrag = _context.Aufträge
                .Select(a => new AuftragDTO
                {
                    AuftragID = a.AuftragID,
                    KundeID = a.KundeID,
                    Dienstleistung = a.Dienstleistung,
                    Priorität = a.Priorität,
                    Status = a.Status,
                    ErstelltAm = a.ErstelltAm
                })
                .FirstOrDefault(a => a.AuftragID == id);

            if (auftrag == null)
            {
                return NotFound(new { message = "Auftrag nicht gefunden." });
            }

            return Ok(auftrag);
        }
    }
}
