namespace Warranty.Core.Entities
{
    using System;

    public class Community : IAuditableEntity
    {
        public virtual Guid CommunityId { get; set; }
        public virtual string CommunityNumber { get; set; }
        public virtual string CommunityName { get; set; }
        public virtual Guid? CityId { get; set; }
        public virtual Guid? DivisionId { get; set; }
        public virtual Guid? ProjectId { get; set; }
        public virtual Guid? SateliteCityId { get; set; }
        public virtual string CommunityStatusCode { get; set; }
        public virtual string CommunityStatusDescription { get; set; }
        public virtual string ProductType { get; set; }
        public virtual string ProductTypeDescription { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
        public virtual string UpdatedBy { get; set; }
    }
}