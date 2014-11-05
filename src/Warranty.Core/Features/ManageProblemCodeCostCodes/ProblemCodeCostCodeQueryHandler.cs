namespace Warranty.Core.Features.ManageProblemCodeCostCodes
{
    using System.Collections.Generic;
    using NPoco;

    public class ProblemCodeCostCodeQueryHandler : IQueryHandler<ProblemCodeCostCodeQuery, IEnumerable<ProblemCodeCostCodeModel>>
    {
        private readonly IDatabase _database;

        public ProblemCodeCostCodeQueryHandler(IDatabase database)
        {
            _database = database;
        }

        public IEnumerable<ProblemCodeCostCodeModel> Handle(ProblemCodeCostCodeQuery query)
        {
            using (_database)
            {
                const string sql = @"SELECT DISTINCT p.JdeCode AS ProblemJdeCode, p.CategoryCode AS ProblemCode, c.CostCode 
                                    FROM ProblemCodes p
                                    LEFT JOIN CityCodeProblemCodeCostCodes c ON c.ProblemJdeCode = p.JdeCode AND c.CityCode = @0";
                var codes = _database.Fetch<ProblemCodeCostCodeModel>(sql, query.CityCode);
                return codes;
            }
        }
    }
}
