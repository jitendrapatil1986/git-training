namespace Warranty.Core.Features.ManageProblemCodeCostCodes
{
    using System.Collections.Generic;

    public class ProblemCodeCostCodeQuery : IQuery<ProblemCodeCostCodeModel>, IQuery<IEnumerable<ProblemCodeCostCodeModel>>
    {
        public string CityCode { get; set; }
    }
}
