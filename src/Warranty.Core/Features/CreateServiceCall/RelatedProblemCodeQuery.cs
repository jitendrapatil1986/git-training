namespace Warranty.Core.Features.CreateServiceCall
{
    using System;
    using System.Collections.Generic;
    using Core;

    public class RelatedProblemCodeQuery : IQuery<IEnumerable<RelatedProblemCode>>
    {
        public Guid JobId { get; set; }
        public string ProblemCode { get; set; }
    }
}