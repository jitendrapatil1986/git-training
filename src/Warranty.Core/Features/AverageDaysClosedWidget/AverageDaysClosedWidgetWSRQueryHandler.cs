namespace Warranty.Core.Features.AverageDaysClosedWidget
{
    using Extensions;
    using NPoco;
    using Common.Security.Session;
    using System;

    public class AverageDaysClosedWidgetWSRQueryHandler : IQueryHandler<AverageDaysClosedWidgetWSRQuery, AverageDaysClosedWidgetModel>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public AverageDaysClosedWidgetWSRQueryHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public AverageDaysClosedWidgetModel Handle(AverageDaysClosedWidgetWSRQuery query)
        {
            var user = _userSession.GetCurrentUser();

            using (_database)
            {
                var sqlAvgDays = @"SELECT AVG(DATEDIFF(DD, sc.CreatedDate, CompletionDate)) as AverageDaysClosed
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
                                              AND CityCode IN ({0})
                                              AND EmployeeNumber=@1
                                              AND sc.ServiceCallType = 'Warranty Service Request'";

                var avgDaysClosedThisMonth = Convert.ToInt32(Math.Round(_database.ExecuteScalar<decimal>(string.Format(sqlAvgDays, user.Markets.CommaSeparateWrapWithSingleQuote()), SystemTime.Today, user.EmployeeNumber), MidpointRounding.AwayFromZero));
                var avgDaysClosedLastMonth = Convert.ToInt32(Math.Round(_database.ExecuteScalar<decimal>(string.Format(sqlAvgDays, user.Markets.CommaSeparateWrapWithSingleQuote()), SystemTime.Today.ToLastDay().AddMonths(-1), user.EmployeeNumber), MidpointRounding.AwayFromZero));

                return new AverageDaysClosedWidgetModel
                           {
                               AvgDaysClosedLastMonth = avgDaysClosedLastMonth,
                               AvgDaysClosedThisMonth = avgDaysClosedThisMonth,
                           };
            }
        }
    }
}