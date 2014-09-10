namespace Warranty.Core.DataAccess.Mappings.Lookups
{
    using Entities.Lookups;

    public class ProblemCodeMap : LookupMap<ProblemCode>
    {
        public ProblemCodeMap() : base("lookups.ProblemCodes", "ProblemCodeId", "ProblemCode")
        {
        }
    }
}