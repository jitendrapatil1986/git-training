using NHibernate.Mapping.ByCode.Conformist;
using Warranty.Core.Entities;

namespace Warranty.Core.DataAccess
{
    public class AuditableEntityMapping<T> : ClassMapping<T> where T : class , IAuditableEntity
    {
        public AuditableEntityMapping()
        {
            Property(x => x.CreatedBy);
            Property(x => x.CreatedDate);
            Property(x => x.UpdatedBy);
            Property(x => x.UpdatedDate);
        }
    }
}