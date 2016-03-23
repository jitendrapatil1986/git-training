namespace Warranty.Core.Services.Models
{
    public class CommunityDetails
    {
        public JdeField Division { get; set; }
        public JdeField Project { get; set; }
        public JdeField Area { get; set; }
        public JdeField Market { get; set; }
        public JdeField ConstructionType { get; set; }
        public JdeField Company { get; set; }
        public JdeField Status { get; set; }
        public JdeField State { get; set; }
        public JdeField ProductType { get; set; }
        public JdeField Community { get; set; }
        public string MarketingName { get; set; }
        public string CommunityClosed { get; set; }
        public bool IsBuilding { get; set; }
        public bool IsActive { get; set; }
    }
}