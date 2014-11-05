namespace Warranty.Core.Features.ManageProblemCodeCostCodes
{
    public class ProblemCodeCostCodeUpdateCommand : ICommand<bool>
    {
        public string CityCode { get; set; }
        public string ProblemJdeCode { get; set; }
        public string CostCode { get; set; }
    }
}
