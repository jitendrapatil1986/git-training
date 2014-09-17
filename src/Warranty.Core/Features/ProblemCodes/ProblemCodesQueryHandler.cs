namespace Warranty.Core.Features.ProblemCodes
{
    using System.Collections.Generic;
    using Entities.Lookups;
    using NPoco;

    public class ProblemCodesQueryHandler : IQueryHandler<ProblemCodesQuery, IEnumerable<ProblemCode>>
    {
        private readonly IDatabase _database;

        public ProblemCodesQueryHandler(IDatabase database)
        {
            _database = database;
        }

        public IEnumerable<ProblemCode> Handle(ProblemCodesQuery query)
        {
            using (_database)
            {
                return _database.Fetch<ProblemCode>();
            }
        }
    }
}