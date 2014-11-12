namespace Warranty.Core.Features.ProblemCodes
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Entities;
    using NPoco;

    public class ProblemCodesQueryHandler : IQueryHandler<ProblemCodesQuery, IEnumerable<SelectListItem>>
    {
        private readonly IDatabase _database;

        public ProblemCodesQueryHandler(IDatabase database)
        {
            _database = database;
        }

        public IEnumerable<SelectListItem> Handle(ProblemCodesQuery query)
        {
            using (_database)
            {
                const string sql = @"SELECT DISTINCT JdeCode as Value
                                        ,CategoryCode as Text
                                FROM ProblemCodes
                                ORDER BY CategoryCode";

                return _database.Fetch<SelectListItem>(sql);
            }
        }
    }
}