namespace SkiServiceAPI.DTOs
{
    public class AuftragDTO
    {
        public int AuftragID { get; set; }
        public int KundeID { get; set; }
        public string Dienstleistung { get; set; }
        public int Priorität { get; set; }
        public string Status { get; set; }
        public DateTime ErstelltAm { get; set; }
    }

}
