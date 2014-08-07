namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;
    using NHibernate.Mapping.ByCode;

    public class DivisionMapping : AuditableEntityMapping<Division>
    {
        public DivisionMapping()
        {
            Table("Divisions");

            Id(x => x.DivisionId, map => map.Generator(new GuidCombGeneratorDef()));
            Property(x => x.DivisionCode);
            Property(x => x.DivisionName);
        }
    }
}