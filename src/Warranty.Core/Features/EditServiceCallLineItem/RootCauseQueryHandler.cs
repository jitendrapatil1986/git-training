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
            return SharedQueries.RootCauses.GetRootCauseList(_database, query.ProblemCode);
        }
    }
}
