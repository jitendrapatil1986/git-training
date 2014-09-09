namespace Warranty.Core.Features.QuickSearch
{
    using System.Collections.Generic;
    using NPoco;
    using Security;
    using Extensions;

    public class QuickSearchJobsQueryHandler : IQueryHandler<QuickSearchJobsQuery, IEnumerable<QuickSearchJobModel>>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public QuickSearchJobsQueryHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public IEnumerable<QuickSearchJobModel> Handle(QuickSearchJobsQuery request)
        {
            var currentuser = _userSession.GetCurrentUser();
            var markets = currentuser.Markets;

            const string sqlTemplate = @"SELECT TOP 10 j.JobId as Id, JobNumber, AddressLine, HomeOwnerName, HomePhone
                                FROM Jobs j
                                INNER JOIN HomeOwners h
                                ON j.CurrentHomeOwnerId = h.HomeOwnerId and HomeOwnerNumber=1
                                INNER JOIN Communities c
                                ON j.CommunityId = c.CommunityId
                                INNER JOIN Cities m
                                ON c.CityId = m.CityId
                                WHERE CityCode IN ({0}) AND JobNumber+AddressLine+HomeOwnerName LIKE '%'+@0+'%'
                                ORDER BY HomeOwnerName";

            var result = _database.Fetch<QuickSearchJobModel>(string.Format(sqlTemplate, markets.CommaSeparateWrapWithSingleQuote()), request.Query);
            return result;
        }
    }
}
