namespace Warranty.Core.Features.WarrantyDollarsSpentWidget
{
    using Extensions;
    using NPoco;
    using Security;

    public class WarrantyDollarsSpentWidgetQueryHandler : IQueryHandler<WarrantyDollarsSpentWidgetQuery, WarrantyDollarsSpentWidgetModel>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public WarrantyDollarsSpentWidgetQueryHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public WarrantyDollarsSpentWidgetModel Handle(WarrantyDollarsSpentWidgetQuery query)
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
                            WHERE CloseDate >= DATEADD(yy, @0, @1)
                            AND CloseDate <= @1
                            AND Ci.CityCode IN ({0})";

                var sqlPayments = @"SELECT SUM(Amount) as TotalDollarsSpent
                                    FROM Payments p
                                        INNER JOIN Jobs j
                                        ON p.JobNumber = j.JobNumber
                                        INNER JOIN Communities c
                                        ON j.CommunityId = c.CommunityId
                                        INNER JOIN Cities cc
                                        ON c.CityId = cc.CityId
                                        INNER JOIN CommunityAssignments ca
                                        ON c.CommunityId = ca.CommunityId
                                        INNER JOIN Employees e
                                        ON ca.EmployeeId = e.EmployeeId
                                    WHERE MONTH(p.CreatedDate) = MONTH(@0) AND YEAR(p.CreatedDate) = YEAR(@0) AND CityCode IN ({0})";

                var numberOfHomesThisMonth = _database.ExecuteScalar<int>(string.Format(sqlNumberOfHomes, user.Markets.CommaSeparateWrapWithSingleQuote()), -2, SystemTime.Today);
                var totalPaymentsThisMonth = _database.ExecuteScalar<decimal>(string.Format(sqlPayments, user.Markets.CommaSeparateWrapWithSingleQuote()), SystemTime.Today);
                var numberOfHomesLastMonth = _database.ExecuteScalar<int>(string.Format(sqlNumberOfHomes, user.Markets.CommaSeparateWrapWithSingleQuote()), -2, SystemTime.Today.ToLastDay().AddMonths(-1));
                var totalPaymentsLastMonth = _database.ExecuteScalar<decimal>(string.Format(sqlPayments, user.Markets.CommaSeparateWrapWithSingleQuote()), SystemTime.Today.AddMonths(-1));

                return new WarrantyDollarsSpentWidgetModel
                           {
                               AmountSpentThisMonth = numberOfHomesThisMonth > 0 ? totalPaymentsThisMonth/numberOfHomesThisMonth : 0,
                               AmountSpentLastMonth = numberOfHomesLastMonth > 0 ? totalPaymentsLastMonth/numberOfHomesLastMonth : 0,
                           };
            }
        }
    }
}