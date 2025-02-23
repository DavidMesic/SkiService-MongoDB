namespace SkiServiceAPI.DTOs
{
    public class AuftragDTO
    {
        public string OrderID { get; set; }
        public string AccountID { get; set; }
        public string Service { get; set; }
        public int Priority { get; set; }
        public string Status { get; set; }
        public DateTime CreatetAt { get; set; }
    }

}
