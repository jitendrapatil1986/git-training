using Warranty.Core.Enumerations;

namespace Warranty.Core.Features.AmountSpentWidget
{
    using System.Collections.Generic;
    using NPoco;
    using Security;
    using Extensions;
    using System.Linq;

    public class AmountSpentWidgetQueryHandler : IQueryHandler<AmountSpentWidgetQuery, AmountSpentWidgetModel>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public AmountSpentWidgetQueryHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public AmountSpentWidgetModel Handle(AmountSpentWidgetQuery query)
        {
            var user = _userSession.GetCurrentUser();

            using (_database)
            {
                return new AmountSpentWidgetModel
                           {
                               Categories = GetCategories(user),
                               SeriesList = GetAmountsForLastSixMonths(user),
                               YearTodate = GetYearToDateAmount(user),
                               MonthTodate  = GetMonthToDateAmount(user),
                               QuarterToDate = GetQuarterToDateAmount(user),

                           };
            }
        }

        private const string AgregationsTemplate = @"SELECT SUM(TotalDollarsSpent)/SUM(NumberOfJobs) as DollarsSpent
                                                    FROM
                                                    (
                                                    SELECT SUM(Amount) TotalDollarsSpent, CityCode 
                                                    FROM Payments p
                                                    INNER JOIN Communities c
                                                    ON left(p.CommunityNumber, 4) = c.CommunityNumber
                                                    INNER JOIN Cities cy
                                                    ON c.CityId = cy.CityId
                                                    WHERE YEAR(p.CreatedDate) = YEAR(getdate()) {0} 
                                                                                            GROUP BY cy.CityCode
                                                                                        ) payments
                                                                                        INNER JOIN 
                                                                                        (
                                                                                            SELECT COUNT(*) as NumberOfJobs, CityCode
                                                                                            FROM Jobs j
                                                                                            INNER JOIN Communities c
                                                                                            ON j.CommunityId = c.CommunityId
                                                                                            INNER JOIN Cities cy
                                                                                            on c.CityId = cy.CityId
                                                                                            GROUP BY CityCode
                                                                                        ) jobs
                                                                                        ON payments.CityCode = jobs.CityCode
                                                                                        WHERE payments.CityCode IN ({1})";

        private decimal GetYearToDateAmount(IUser user)
        {
            var markets = user.Markets;

            var query = string.Format(AgregationsTemplate, string.Empty, markets.CommaSeparateWrapWithSingleQuote());
            
            var result = _database.Single<decimal?>(query);
            
            return result ?? 0;
        }

        private decimal GetQuarterToDateAmount(IUser user)
        {
            var markets = user.Markets;

            var query = string.Format(AgregationsTemplate, "AND DATEPART(qq, p.CreatedDate) = DATEPART(qq, getdate())", markets.CommaSeparateWrapWithSingleQuote());

            var result = _database.Single<decimal?>(query);

            return result ?? 0;
        }

        private decimal GetMonthToDateAmount(IUser user)
        {
            var markets = user.Markets;

            var query = string.Format(AgregationsTemplate, "AND MONTH(p.CreatedDate) = MONTH(getdate())", markets.CommaSeparateWrapWithSingleQuote());

            var result = _database.Single<decimal?>(query);

            return result ?? 0;
        }

        private List<AmountSpentWidgetModel.Series> GetAmountsForLastSixMonths(IUser user)
        {
            var markets = user.Markets;
            var listSeries = new List<AmountSpentWidgetModel.Series>();

            const string sql = @"SELECT SUM(Amount) TotalDollarsSpent
                            FROM Payments p
                            INNER JOIN Communities c
                                ON left(p.CommunityNumber, 4) = c.CommunityNumber
                            INNER JOIN Cities cy
                                ON c.CityId = cy.CityId
                            WHERE  cy.CityCode IN ({0}) AND
                                YEAR(p.CreatedDate) = YEAR(getdate()) and p.CreatedDate >= DATEADD(MONTH, -6, GETDATE()) and p.createddate <= getdate()
                                GROUP BY MONTH(p.CreatedDate)";

                var result = _database.Fetch<decimal>(string.Format(sql, markets.CommaSeparateWrapWithSingleQuote()));
                listSeries.Add(new AmountSpentWidgetModel.Series
                    {
                        Data = result,
                        Name = string.Format("My Divisions ({0})", markets.CommaSeparate())
                    });

            return listSeries;
        }

        private string[] GetCategories(IUser user)
        {
            var markets = user.Markets;

            const string sql = @"SELECT MONTH(p.CreatedDate)
                        FROM Payments p
                        INNER JOIN Communities c
                            ON left(p.CommunityNumber, 4) = c.CommunityNumber
                        INNER JOIN Cities cy
                            ON c.CityId = cy.CityId
                        WHERE  cy.CityCode IN ({0}) AND
                            YEAR(p.CreatedDate) = YEAR(getdate()) and p.CreatedDate >= DATEADD(MONTH, -6, GETDATE()) and p.createddate <= getdate()
                            GROUP BY MONTH(p.CreatedDate)";
            var result = _database.Fetch<int>(string.Format(sql, markets.CommaSeparateWrapWithSingleQuote()));
            return result.Select(x => Month.FromValue(x).Abbreviation).ToArray();
        }
    }
}
