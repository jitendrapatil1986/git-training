namespace Warranty.Core.Features.QuickSearch
{
    using System.Collections.Generic;
    using Extensions;
    using NPoco;
    using Security;

    public class QuickSearchCallsQueryHandler : IQueryHandler<QuickSearchCallsQuery, IEnumerable<QuickSearchCallModel>>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public QuickSearchCallsQueryHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public IEnumerable<QuickSearchCallModel> Handle(QuickSearchCallsQuery request)
        {
            var currentuser = _userSession.GetCurrentUser();
            var markets = currentuser.Markets;

            const string sqlTemplate = @"select 
                                            REPLACE((SELECT li.problemcode + ', '
                                                    FROM ServiceCallLineItems li WHERE li.ServiceCallId = c.servicecallid
                                                    FOR xml path('')) + ';', ', ;', '') AS ProblemCodes,
                                            c.ServiceCallId as Id, JobNumber, AddressLine, HomeOwnerName, HomePhone
                                            from ServiceCalls c
                                            inner join Jobs j
                                            on c.JobId = j.JobId
                                            inner join HomeOwners ho
                                            on j.CurrentHomeOwnerId = ho.HomeOwnerId
                                            inner join Communities co
                                            on j.CommunityId = co.CommunityId
                                            inner join Cities cy
                                            on co.CityId = cy.CityId
                                            WHERE CityCode IN ({0}) AND JobNumber+AddressLine+HomeOwnerName LIKE '%'+@0+'%'";

            var result = _database.Fetch<QuickSearchCallModel>(string.Format(sqlTemplate, markets.CommaSeparateWrapWithSingleQuote()), request.Query);
            return result;
        }
    }
}