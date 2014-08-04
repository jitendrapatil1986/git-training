using NHibernate.Mapping.ByCode.Conformist;
using Warranty.Core.Entities;

namespace Warranty.Core.DataAccess.Mappings
{
    public class DivisionMapping : ClassMapping<Division>
    {
        public DivisionMapping()
        {
            Table("Divisions");

            Id(x => x.DivisionId);
            Property(x => x.DivisionCode);
            Property(x => x.DivisionName);
            Property(x => x.CreatedDate);
            Property(x => x.CreatedBy);
            Property(x => x.UpdatedDate);
            Property(x => x.UpdatedBy);
        }
    }
}