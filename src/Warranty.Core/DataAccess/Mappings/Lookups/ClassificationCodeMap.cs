namespace Warranty.Core.DataAccess.Mappings.Lookups
{
    using Entities.Lookups;

    public class ClassificationCodeMap : LookupMap<ClassificationCode>
    {
        public ClassificationCodeMap() : base("lookups.ClassificationCodes", "ClassificationCodeId", "ClassificationCode")
        {
        }
    }
}