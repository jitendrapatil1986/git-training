namespace Warranty.Core.Features.ManageProblemCodeCostCodes
{
    using NPoco;

    public class ProblemCodeCostCodeUpdateCommandHandler : ICommandHandler<ProblemCodeCostCodeUpdateCommand>
    {
        private readonly IDatabase _database;

        public ProblemCodeCostCodeUpdateCommandHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(ProblemCodeCostCodeUpdateCommand message)
        {
            using (_database)
            {
                const string select = @"SELECT COUNT(*) FROM CityCodeProblemCodeCostCodes
                                        WHERE CityCode = @0 AND ProblemJdeCode = @1";
                
                var exists = _database.Single<int>(select, message.CityCode, message.ProblemJdeCode);

                if (exists >= 1)
                {
                    //update
                }
                else
                {
                    //insert
                }
            }
        }
    }
}
