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

        public IEnumerable<QuickSearchJobModel> Handle(QuickSearchJobsQuery query)
        {
            var currentuser = _userSession.GetCurrentUser();
            var markets = currentuser.Markets;

            const string sqlTemplate = @"SELECT TOP 20 j.JobId as Id, JobNumber, AddressLine, HomeOwnerName, HomeOwnerNumber, HomePhone, EmailAddress
                                FROM Jobs j
                                LEFT JOIN HomeOwners h
                                ON j.CurrentHomeOwnerId = h.HomeOwnerId and HomeOwnerNumber=(select max(homeownernumber) from HomeOwners WHERE JobId=j.JobId)
                                INNER JOIN Communities c
                                ON j.CommunityId = c.CommunityId
                                INNER JOIN Cities m
                                ON c.CityId = m.CityId
                                WHERE CityCode IN ({0}) AND JobNumber+AddressLine+ISNULL(HomeOwnerName, '')+ ISNULL(HomePhone, '') + ISNULL(EmailAddress, '')  LIKE '%'+ @0 +'%' 
                                AND j.Stage >= 3
                                ORDER BY HomeOwnerName";

            var result = _database.Fetch<QuickSearchJobModel>(string.Format(sqlTemplate, markets.CommaSeparateWrapWithSingleQuote()), query.Query);
            result.ForEach(x => x.HomePhone = x.HomePhone.ToPhoneNumberWithExtension());
            return result;
        }
    }
}
