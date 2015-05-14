namespace Warranty.Core.Features.Report.MailMerge
{
    using Extensions;
    using NPoco;
    using Common.Security.User.Session;

    public class MailMergeQueryHandler : IQueryHandler<MailMergeQuery, MailMergeQuery>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public MailMergeQueryHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public MailMergeQuery Handle(MailMergeQuery query)
        {
            if (!query.Date.HasValue)
                return query;

            using (_database)
            {
                var user = _userSession.GetCurrentUser();
                var sql = @"SELECT 
                                        ho.HomeownerName
                                        , j.AddressLine
                                        , j.City
                                        , j.StateCode
                                        , j.PostalCode
                                        , ho.HomePhone
                                        , c.CommunityName
                                        , CONVERT(varchar, j.CloseDate,101) CloseDate
                                    FROM Homeowners ho
                                        INNER JOIN Jobs j
	                                       ON j.CurrentHomeownerId = ho.HomeownerId
                                        INNER JOIN Communities c
	                                       ON j.CommunityId = c.CommunityId
                                        INNER JOIN Cities cc
	                                       ON cc.CityId = c.CityId
                                    WHERE YEAR(j.CloseDate) = YEAR(@0) AND MONTH(j.CloseDate) = MONTH(@0)
                                        AND cc.CityCode in ({0})
                                        ORDER BY j.CloseDate";
                
                
                sql = string.Format(sql, user.Markets.CommaSeparateWrapWithSingleQuote());

                var mailMergeReport = new MailMergeReport
                    {
                        Customers = _database.Fetch<MailMergeReport.Customer>(sql, query.Date),
                    };
                
                query.Result = mailMergeReport;
                
                return query;
            }
        }
    }
}
