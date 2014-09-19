using NPoco.FluentMappings;
using Warranty.Core.Entities;

namespace Warranty.Core.DataAccess.Mappings
{
    public class AuditCreatedEntityMap<T> : Map<T> where T : class, IAuditCreatedEntity
    {
        public AuditCreatedEntityMap()
        {
            Columns(x =>
                {
                    x.Column(y => y.CreatedDate);
                    x.Column(y => y.CreatedBy);
                });
        }
    }
}