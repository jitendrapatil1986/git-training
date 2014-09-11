namespace Warranty.Core.Features.CreateServiceCall
{
    using System;

    public class RelatedProblemCode
    {
        public Guid ServiceCallId { get; set; }
        public string CallNumber { get; set; }
        public string ProblemDescription { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}