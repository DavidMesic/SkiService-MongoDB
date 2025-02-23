using SkiServiceAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Auftrag
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AuftragID { get; set; }

    [Required]
    [ForeignKey("Kunde")]
    public int KundeID { get; set; }

    [JsonIgnore]
    public Account? Kunde { get; set; }

    [Required]
    [MaxLength(50)]
    public string Dienstleistung { get; set; }

    [Required]
    [Range(1, 3)]
    public int Priorität { get; set; }

    [MaxLength(20)]
    [JsonIgnore]
    public string Status { get; set; } = "Offen";

    [JsonIgnore]
    public DateTime ErstelltAm { get; set; } = DateTime.Now;
}
