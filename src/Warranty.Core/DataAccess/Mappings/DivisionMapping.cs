using NHibernate.Mapping.ByCode;
using Warranty.Core.Entities;

namespace Warranty.Core.DataAccess.Mappings
{
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