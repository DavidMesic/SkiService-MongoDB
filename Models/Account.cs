using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Account
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AccountID { get; set; }

    [Required]
    [MaxLength(50)]
    public string Benutzername { get; set; }

    [Required]
    public string PasswortHash { get; set; }

    [Required]
    [MaxLength(100)]
    public string Email { get; set; }

    [MaxLength(20)]
    public string Telefon { get; set; } = null;

    [MaxLength(20)]
    [JsonIgnore]
    public string Rolle { get; set; } = "Kunde";

    [JsonIgnore]
    public ICollection<Auftrag>? Aufträge { get; set; }
}
