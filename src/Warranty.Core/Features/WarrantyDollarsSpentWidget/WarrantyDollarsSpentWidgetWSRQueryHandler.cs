namespace Warranty.Core.Features.WarrantyDollarsSpentWidget
{
    using Enumerations;
    using Extensions;
    using NPoco;
    using Common.Security.Session;

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

                var sqlPayments = @"; WITH AllWarrantyPayments (FiscalYear, Month, Year, CostCenter, ObjectAccount, Amount, CommunityNumber, FirstDayOfMonth, LastDayOfMonth)
                                        AS
                                        (
                                            SELECT *, 
                                                SUBSTRING(CostCenter, 1, 4) as CommunityNumber,
                                                CONVERT(DATE, CONVERT(VARCHAR, Month) + '-1-' + CONVERT(VARCHAR, Year)) as FirstDayOfMonth ,
                                                DATEADD(DD, -1, DATEADD(MM, 1, CONVERT(DATE, CONVERT(VARCHAR, Month) + '-1-' + CONVERT(VARCHAR, Year)))) as LastDayOfMonth 
                                            FROM
                                            (
                                                SELECT GBFY as FiscalYear,
                                                    CASE WHEN MonthAbbr = 'JAN' THEN 1
                                                        WHEN MonthAbbr = 'FEB' THEN 2
                                                        WHEN MonthAbbr = 'MAR' THEN 3
                                                        WHEN MonthAbbr = 'APR' THEN 4
                                                        WHEN MonthAbbr = 'MAY' THEN 5
                                                        WHEN MonthAbbr = 'JUN' THEN 6
                                                        WHEN MonthAbbr = 'JUL' THEN 7
                                                        WHEN MonthAbbr = 'AUG' THEN 8
                                                        WHEN MonthAbbr = 'SEP' THEN 9
                                                        WHEN MonthAbbr = 'OCT' THEN 10
                                                        WHEN MonthAbbr = 'NOV' THEN 11
                                                        WHEN MonthAbbr = 'DEC' THEN 12
                                                        ELSE 0
                                                    END as Month,
                                                    CASE WHEN LEN(GBFY) = 1 THEN RIGHT('200' + CAST(GBFY as VARCHAR(4)), 4)
                                                        WHEN LEN(GBFY) = 2 THEN RIGHT('20' + CAST(GBFY as VARCHAR(4)), 4)
                                                        ELSE 0
                                                    END as Year,
                                                    LTRIM(GBMCU) as CostCenter,
                                                    GBOBJ as ObjectAccount,
                                                    Amount
                                                FROM tmp_JDE_GL_War_Buckets
                                                UNPIVOT (Amount
                                                FOR MonthAbbr IN (JAN, FEB, MAR, APR, MAY, JUN, JUL, AUG, SEP, OCT, NOV, [DEC]))
                                                AS UNPVTTable
                                            ) a
                                        )

                                        SELECT SUM(Amount) as TotalDollarsSpent
                                        FROM
                                        (
                                        SELECT DISTINCT p.* FROM AllWarrantyPayments p
                                        INNER JOIN Communities c
                                            ON p.CommunityNumber = c.CommunityNumber
                                        INNER JOIN Jobs j
                                            ON c.CommunityId = j.CommunityId
                                        INNER JOIN Cities cc
                                            ON c.CityId = cc.CityId
                                        INNER JOIN CommunityAssignments ca
                                            ON c.CommunityId = ca.CommunityId
                                        INNER JOIN Employees e
                                            ON ca.EmployeeId = e.EmployeeId
                                            AND EmployeeNumber=@1
                                        WHERE
                                            cc.CityCode IN ({0})
                                            AND p.FirstDayOfMonth <= @0
                                            AND p.LastDayOfMonth >= @0
                                            AND p.LastDayOfMonth <= DATEADD(yy, 2, j.CloseDate)
                                        ) a";

                var numberOfHomesThisMonth = _database.ExecuteScalar<int>(string.Format(sqlNumberOfHomes, user.Markets.CommaSeparateWrapWithSingleQuote()), -2, SystemTime.Today, user.EmployeeNumber);
                var totalPaymentsThisMonth = _database.ExecuteScalar<decimal>(string.Format(sqlPayments, user.Markets.CommaSeparateWrapWithSingleQuote()), SystemTime.Today, user.EmployeeNumber);
                var numberOfHomesLastMonth = _database.ExecuteScalar<int>(string.Format(sqlNumberOfHomes, user.Markets.CommaSeparateWrapWithSingleQuote()), -2, SystemTime.Today.ToLastDay().AddMonths(-1), user.EmployeeNumber);
                var totalPaymentsLastMonth = _database.ExecuteScalar<decimal>(string.Format(sqlPayments, user.Markets.CommaSeparateWrapWithSingleQuote()), SystemTime.Today.AddMonths(-1), user.EmployeeNumber);

                return new WarrantyDollarsSpentWidgetModel
                           {
                               AmountSpentThisMonth = numberOfHomesThisMonth > 0 ? totalPaymentsThisMonth/numberOfHomesThisMonth : 0,
                               AmountSpentLastMonth = numberOfHomesLastMonth > 0 ? totalPaymentsLastMonth/numberOfHomesLastMonth : 0,
                           };
            }
        }
    }
}