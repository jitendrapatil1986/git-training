namespace Warranty.Core.Features.ManageProblemCodeCostCodes
{
    using Entities;
    using NPoco;

    public class ProblemCodeCostCodeUpdateCommandHandler : ICommandHandler<ProblemCodeCostCodeUpdateCommand, bool>
    {
        private readonly IDatabase _database;

        public ProblemCodeCostCodeUpdateCommandHandler(IDatabase database)
        {
            _database = database;
        }

        public bool Handle(ProblemCodeCostCodeUpdateCommand message)
        {
            using (_database)
            {
                const string select = @"SELECT * FROM CityCodeProblemCodeCostCodes
                                        WHERE CityCode = @0 AND ProblemJdeCode = @1";
                
                var existing = _database.SingleOrDefault<CityCodeProblemCodeCostCode>(select, message.CityCode, message.ProblemJdeCode);

                if (existing != null)
                {
                    existing.CostCode = message.CostCode;
                    _database.Update(existing);
                }
                else
                {
                    var newCode = new CityCodeProblemCodeCostCode()
                    {
                        CityCode = message.CityCode,
                        CostCode = message.CostCode,
                        ProblemJdeCode = message.ProblemJdeCode
                    };
                    _database.Insert(newCode);
                }
            }
            return true;
        }
    }
}
