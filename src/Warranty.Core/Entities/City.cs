using System;

namespace Warranty.Core.Entities
{
    public class City : IAuditableEntity
    {
        public virtual Guid CityId { get; set; }
        public virtual string CityCode { get; set; }
        public virtual string CityName { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
        public virtual string UpdatedBy { get; set; }
    }
}