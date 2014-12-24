namespace Warranty.Core.Features.ManageProblemCodeCostCodes
{
    using System.Web.Mvc;
    using NPoco;

    public class ManageProblemCodeCostCodeQueryHandler : IQueryHandler<ManageProblemCodeCostCodeQuery, ManageProblemCodeCostCodeModel>
    {
        private readonly IDatabase _database;

        public ManageProblemCodeCostCodeQueryHandler(IDatabase database)
        {
            _database = database;
        }

        public ManageProblemCodeCostCodeModel Handle(ManageProblemCodeCostCodeQuery query)
        {
            using (_database)
            {
                const string sql = @"SELECT CityCode AS Value, CityName AS Text FROM Cities WHERE CityCode IS NOT NULL AND CityCode <> ''";
                var cities = _database.Fetch<SelectListItem>(sql);

                return new ManageProblemCodeCostCodeModel()
                {
                    Cities = cities
                };
            }
        }
    }
}
