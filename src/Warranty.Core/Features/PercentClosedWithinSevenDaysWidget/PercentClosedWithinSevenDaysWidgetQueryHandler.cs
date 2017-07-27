namespace Warranty.Core.Features.PercentClosedWithinSevenDaysWidget
{
    using Extensions;
    using NPoco;
    using Common.Security.Session;
    using System;

    public class PercentClosedWithinSevenDaysWidgetQueryHandler : IQueryHandler<PercentClosedWithinSevenDaysWidgetQuery, PercentClosedWithinSevenDaysWidgetModel>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public PercentClosedWithinSevenDaysWidgetQueryHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public PercentClosedWithinSevenDaysWidgetModel Handle(PercentClosedWithinSevenDaysWidgetQuery query)
        {
            var user = _userSession.GetCurrentUser();

            using (_database)
            {
                var sqlPercentClosed = @"SELECT SUM(CASE WHEN DATEDIFF(DD, sc.CreatedDate, CompletionDate) <= 7 THEN 1 ELSE 0 END) * 100.0/COUNT(*) as PercentClosedWithin7Days
                                            FROM ServiceCalls sc
                                            INNER JOIN Employees e
                                            ON sc.WarrantyRepresentativeEmployeeId = e.EmployeeId
                                            INNER JOIN Jobs j
                                            ON sc.JobId = j.JobId
                                            INNER JOIN Communities c
                                            ON j.CommunityId = c.CommunityId
                                            INNER JOIN Cities cc
                                            ON c.CityId = cc.CityId
                                            WHERE MONTH(CompletionDate) = MONTH(@0) AND YEAR(CompletionDate) = YEAR(@0)
                                              AND CityCode IN ({0})";

                var percentClosedThisMonth = Convert.ToInt32(Math.Round(_database.ExecuteScalar<decimal>(string.Format(sqlPercentClosed, user.Markets.CommaSeparateWrapWithSingleQuote()), SystemTime.Today), MidpointRounding.AwayFromZero));
                var percentClosedLastMonth = Convert.ToInt32(Math.Round(_database.ExecuteScalar<decimal>(string.Format(sqlPercentClosed, user.Markets.CommaSeparateWrapWithSingleQuote()), SystemTime.Today.ToLastDay().AddMonths(-1)), MidpointRounding.AwayFromZero));

                return new PercentClosedWithinSevenDaysWidgetModel
                           {
                               PercentClosedLastMonth = percentClosedLastMonth,
                               PercentClosedThisMonth = percentClosedThisMonth,
                           };
            }
        }
    }
}