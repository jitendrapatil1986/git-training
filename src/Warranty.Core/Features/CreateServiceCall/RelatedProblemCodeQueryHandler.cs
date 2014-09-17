namespace Warranty.Core.Features.CreateServiceCall
{
    using System.Collections.Generic;
    using Core;
    using NPoco;

    public class RelatedProblemCodeQueryHandler : IQueryHandler<RelatedProblemCodeQuery, IEnumerable<RelatedProblemCode>>
    {
        private readonly IDatabase _database;

        public RelatedProblemCodeQueryHandler(IDatabase database)
        {
            _database = database;
        }

        public IEnumerable<RelatedProblemCode> Handle(RelatedProblemCodeQuery query)
        {
            using (_database)
            {
                var sql = @"select ServiceCallNumber as CallNumber, sc.ServiceCallId, ProblemDescription, sc.CreatedDate
                            from ServiceCalls sc
                            inner join ServiceCallLineItems li
                            on sc.ServiceCallId = li.ServiceCallId
                            where JobId=@0 and ProblemCode=@1";

                return _database.Fetch<RelatedProblemCode>(sql, query.JobId, query.ProblemCode);
            }
        }
    }
}