namespace Warranty.Core.DataAccess.Mappings.Lookups
{
    using Entities.Lookups;

    public class RootCauseMap : LookupMap<RootCause>
    {
        public RootCauseMap() : base("lookups.RootCauses", "RootCauseId", "RootCause")
        {
        }
    }
}