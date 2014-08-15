using Warranty.Core.Entities;

namespace Warranty.Core.DataAccess.Mappings
{
    using NPoco.FluentMappings;

    public class AuditableEntityMap<T> : Map<T> where T : class, IAuditableEntity
    {
        public AuditableEntityMap()
        {
            Columns(x =>
                        {
                            x.Column(y => y.CreatedDate);
                            x.Column(y => y.CreatedBy);
                            x.Column(y => y.UpdatedDate);
                            x.Column(y => y.UpdatedBy);
                        });
        }
    }
}
