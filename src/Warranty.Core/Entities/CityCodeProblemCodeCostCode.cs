using System;

namespace Warranty.Core.Entities
{
    public class CityCodeProblemCodeCostCode : IAuditableEntity
    {
        public virtual Guid CityCodeProblemCodeCostCodeId { get; set; }
        public virtual string CityCode { get; set; }
        public virtual string ProblemJdeCode { get; set; }
        public virtual string CostCode { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string UpdatedBy { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
    }
}
