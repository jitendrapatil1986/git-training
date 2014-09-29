namespace Warranty.Core.Features.WarrantyDollarsSpentWidget
{
    using Extensions;
    using NPoco;
    using Security;

    public class WarrantyDollarsSpentWidgetWSRQueryHandler : IQueryHandler<WarrantyDollarsSpentWidgetWSRQuery, WarrantyDollarsSpentWidgetModel>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public WarrantyDollarsSpentWidgetWSRQueryHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public WarrantyDollarsSpentWidgetModel Handle(WarrantyDollarsSpentWidgetWSRQuery query)
        {
            var user = _userSession.GetCurrentUser();

            using (_database)
            {
                var sqlNumberOfHomes = @"SELECT COUNT(*) as NumberOfWarrantableHomes
                            FROM Jobs j
                            INNER JOIN Communities c
                            ON j.CommunityId = c.CommunityId
                            INNER JOIN Cities Ci
                            ON c.CityId = Ci.CityId
                            INNER JOIN CommunityAssignments ca
                            ON c.CommunityId = ca.CommunityId
                            INNER JOIN Employees e
                            ON ca.EmployeeId = e.EmployeeId
                            WHERE CloseDate >= DATEADD(yy, @0, @1)
                            AND CloseDate <= @1
                            AND Ci.CityCode IN ({0})
                            AND EmployeeNumber=@2";

                            //TODO: Rewrite to use WarrantyDollars
                var sqlPayments = @"SELECT SUM(p.Amount) as TotalDollarsSpent
                                    FROM Jobs j
                                    INNER JOIN Communities c
                                    ON j.CommunityId = c.CommunityId
                                    INNER JOIN Cities Ci
                                    ON c.CityId = Ci.CityId
                                    INNER JOIN Payments p
                                    ON j.JobNumber = p.JobNumber
                                    WHERE Ci.CityCode IN ({0}) AND MONTH(p.CreatedDate) = @0 AND YEAR(p.CreatedDate) = @1";

                var numberOfHomesThisMonth = _database.ExecuteScalar<int>(string.Format(sqlNumberOfHomes, user.Markets.CommaSeparateWrapWithSingleQuote()), -2, SystemTime.Today, user.EmployeeNumber);
                var totalPaymentsThisMonth = _database.ExecuteScalar<decimal>(string.Format(sqlPayments, user.Markets.CommaSeparateWrapWithSingleQuote()), SystemTime.Today.Month, SystemTime.Today.Year);
                var numberOfHomesLastMonth = _database.ExecuteScalar<int>(string.Format(sqlNumberOfHomes, user.Markets.CommaSeparateWrapWithSingleQuote()), -2, SystemTime.Today.ToLastDay().AddMonths(-1), user.EmployeeNumber);
                var totalPaymentsLastMonth = _database.ExecuteScalar<decimal>(string.Format(sqlPayments, user.Markets.CommaSeparateWrapWithSingleQuote()), SystemTime.Today.AddMonths(-1).Month, SystemTime.Today.AddMonths(-1).Year);

                return new WarrantyDollarsSpentWidgetModel
                           {
                               AmountSpentThisMonth = totalPaymentsThisMonth/numberOfHomesThisMonth,
                               AmountSpentLastMonth = totalPaymentsLastMonth/numberOfHomesLastMonth,
                           };
            }
        }
    }
}