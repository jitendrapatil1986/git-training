namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;

    public class DivisionMapping : AuditableEntityMapping<Division>
    {
        public DivisionMapping()
        {
            Table("Divisions");

            Id(x => x.DivisionId);
            Property(x => x.DivisionCode);
            Property(x => x.DivisionName);
        }
    }
}