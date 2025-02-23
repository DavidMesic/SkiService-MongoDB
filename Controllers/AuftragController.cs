using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkiServiceAPI.DTOs;
using SkiServiceAPI.Models;
using MongoDB.Driver;
using SkiServiceAPI.Services;

namespace SkiServiceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuftragController : ControllerBase
    {
        private readonly ILogger<AuftragController> _logger;
        private readonly IMongoCollection<Auftrag> _auftraege;
        private readonly IMongoCollection<Account> _accounts;

        public AuftragController(ILogger<AuftragController> logger, MongoDbService mongoDbService)
        {
            _logger = logger;

            _auftraege = mongoDbService.Database.GetCollection<Auftrag>("auftrag");
            _accounts = mongoDbService.Database.GetCollection<Account>("account");
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] Auftrag auftrag)
        {
            if (auftrag == null)
            {
                return BadRequest(new { message = "Ungültige Daten!" });
            }

            // Kunde überprüfen
            var kunde = _accounts.Find(a => a.AccountID == auftrag.KundeID).FirstOrDefault();
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

            // Auftrag in die MongoDB Collection einfügen
            _auftraege.InsertOne(auftrag);
            return Ok(new { message = "Auftrag erfolgreich erstellt!", auftrag });
        }

        // PUT: api/auftrag/update/{id}
        [HttpPut("update/{id}")]
        [Authorize(Roles = "Admin,Mitarbeiter")]
        public IActionResult Update(string id, [FromBody] Auftrag updatedAuftrag)
        {
            var existingAuftrag = _auftraege.Find(a => a.AuftragID == id).FirstOrDefault();
            if (existingAuftrag == null)
            {
                return NotFound(new { message = "Auftrag nicht gefunden." });
            }

            if (ModelState.IsValid)
            {
                existingAuftrag.Dienstleistung = updatedAuftrag.Dienstleistung;
                existingAuftrag.Priorität = updatedAuftrag.Priorität;
                existingAuftrag.Status = updatedAuftrag.Status;

                // Dokument in der Collection ersetzen
                _auftraege.ReplaceOne(a => a.AuftragID == id, existingAuftrag);
                return Ok(new { message = "Auftrag erfolgreich aktualisiert.", updatedAuftrag });
            }

            return BadRequest(new { message = "Ungültige Daten!" });
        }

        // DELETE: api/auftrag/delete/{id}
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin,Mitarbeiter")]
        public IActionResult Delete(string id)
        {
            var auftrag = _auftraege.Find(a => a.AuftragID == id).FirstOrDefault();
            if (auftrag == null)
            {
                return NotFound(new { message = "Auftrag nicht gefunden." });
            }

            var result = _auftraege.DeleteOne(a => a.AuftragID == id);
            return Ok(new { message = "Auftrag erfolgreich gelöscht." });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var auftrag = _auftraege
                .Find(a => a.AuftragID == id)
                .Project(a => new AuftragDTO
                {
                    AuftragID = a.AuftragID,
                    KundeID = a.KundeID,
                    Dienstleistung = a.Dienstleistung,
                    Priorität = a.Priorität,
                    Status = a.Status,
                    ErstelltAm = a.ErstelltAm
                })
                .FirstOrDefault();

            if (auftrag == null)
            {
                return NotFound(new { message = "Auftrag nicht gefunden." });
            }

            return Ok(auftrag);
        }
    }
}
