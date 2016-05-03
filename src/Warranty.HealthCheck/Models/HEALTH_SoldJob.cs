namespace Warranty.HealthCheck.Models
{
    public static class Systems
    {
        public const int TIPS = 1;
        public const int Warranty = 2;
    }

    public class HEALTH_SoldJob
    {
        public string JobNumber { get; set; } 
        public int System { get; set; }
    }
}