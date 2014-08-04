using System;

namespace Warranty.Core.Entities
{
    public class Community : IAuditableEntity
    {
        public virtual int CommunityId { get; set; }
        public virtual string CommunityNumber { get; set; }
        public virtual string CommunityName { get; set; }
        public virtual int CityId { get; set; }
        public virtual int DivisionId { get; set; }
        public virtual int ProjectId { get; set; }
        public virtual int SateliteCityId { get; set; }
        public virtual string CommunityStatusCode { get; set; }
        public virtual string CommunityStatusDescription { get; set; }
        public virtual string ProductType { get; set; }
        public virtual string PlanType { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
        public virtual string UpdatedBy { get; set; }
    }
}