namespace Warranty.Core.Features.EditServiceCallLineItem
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using NPoco;

    public class RootCauseQueryHandler : IQueryHandler<RootCauseQuery, IEnumerable<SelectListItem>>
    {
        private readonly IDatabase _database;

        public RootCauseQueryHandler(IDatabase database)
        {
            _database = database;
        }

        public IEnumerable<SelectListItem> Handle(RootCauseQuery query)
        {
            const string sql = @"SELECT  DetailCode as Value
                                        ,DetailCode as Text
                                FROM ProblemCodes
                                WHERE JdeCode = @0
                                ORDER BY DetailCode";

            return _database.Fetch<SelectListItem>(sql, query.ProblemJdeCode);
        }
    }
}
