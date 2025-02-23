namespace SkiServiceAPI.DTOs
{
    public class AuftragDTO
    {
        public string AuftragID { get; set; }
        public string KundeID { get; set; }
        public string Dienstleistung { get; set; }
        public int Priorität { get; set; }
        public string Status { get; set; }
        public DateTime ErstelltAm { get; set; }
    }

}
