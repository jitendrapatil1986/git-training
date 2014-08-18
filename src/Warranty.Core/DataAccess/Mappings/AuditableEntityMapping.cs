namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;
    using NHibernate.Mapping.ByCode.Conformist;
    
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