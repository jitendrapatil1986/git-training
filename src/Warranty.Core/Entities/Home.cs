using Warranty.Core.ValueObjects;

namespace Warranty.Core.Entities
{
    public class Home
    {
        public virtual long Id { get; set; }
        public virtual HomeOwner Owner { get; set; }
        
        //Site details
        public virtual Address Address { get; set; }
        public virtual string LegalDescription { get; set; }

        //Job Information
        public virtual TeamMember Builder { get; set; }
        public virtual Division Division { get; set; }
        public virtual Project Project { get; set; }
        public virtual TeamMember SalesConsultant { get; set; }

        //Structure Information
        public virtual string Plan { get; set; }
        public virtual string Elevation { get; set; }
        public virtual string Swing { get; set; }
    }
}