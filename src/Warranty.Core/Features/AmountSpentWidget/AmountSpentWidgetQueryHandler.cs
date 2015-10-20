using Warranty.Core.Enumerations;

namespace Warranty.Core.Features.AmountSpentWidget
{
    using System;
    using System.Collections.Generic;
    using NPoco;
    using Common.Security.Session;
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
                               Categories = GetCategories(),
                               SeriesList = GetAmountsForLastSixMonths(user),
                               YearToDate = GetYearToDateAmount(user),
                               MonthToDate  = GetMonthToDateAmount(user),
                               QuarterToDate = GetQuarterToDateAmount(user),

                           };
            }
        }

        private IEnumerable<string> GetCategories()
        {
            var currentQuarter = Month.FromValue(DateTime.Today.Month).Quarter;
            return Month.GetAll().Where(x => x.Quarter == currentQuarter).Select(x => x.Abbreviation);
        }

        private decimal GetYearToDateAmount(IUser user)
        {
            const string whereClause = "WHERE YEAR(p.CreatedDate) = YEAR(getdate())";
            return GetDollarsSpent(whereClause, user);
        }

        private decimal GetQuarterToDateAmount(IUser user)
        {
            const string whereClause = "WHERE YEAR(p.CreatedDate) = YEAR(getdate()) AND DATEPART(qq, p.CreatedDate) = DATEPART(qq, getdate())";
            return GetDollarsSpent(whereClause, user);
        }

        private decimal GetMonthToDateAmount(IUser user)
        {
            const string whereClause = "WHERE YEAR(p.CreatedDate) = YEAR(getdate()) AND MONTH(p.CreatedDate) = MONTH(getdate())";
            return GetDollarsSpent(whereClause, user);
        }

        private decimal GetDollarsSpent(string whereClause, IUser user)
        {
            const string sqlTemplate = @"SELECT SUM(TotalDollarsSpent)/SUM(NumberOfJobs) as DollarsSpent
                                              FROM
                                              (
                                              SELECT SUM(Amount) TotalDollarsSpent, CityCode 
                                                  FROM Payments p
                                                  INNER JOIN Communities c
                                                  ON left(p.CommunityNumber, 4) = c.CommunityNumber
                                                  INNER JOIN Cities cy
                                                  ON c.CityId = cy.CityId
                                              {0} /* WHERE CLAUSE */
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

            var query = string.Format(sqlTemplate, whereClause, user.Markets.CommaSeparateWrapWithSingleQuote());
            var result = _database.Single<decimal?>(query);

            return result ?? 0M;
        }

        private List<AmountSpentWidgetModel.Series> GetAmountsForLastSixMonths(IUser user)
        {
            const string sql = @"SELECT AmountSpent/NumberOfJobs as Amount, jobs.DivisionCode, month
                                    FROM
                                    (
                                        SELECT COUNT(*) as NumberOfJobs, d.DivisionCode
                                        FROM Jobs j
                                        INNER JOIN Communities c
                                        ON j.CommunityId = c.CommunityId
                                        INNER JOIN Divisions d
                                        ON c.DivisionId = d.DivisionId
                                        GROUP BY d.DivisionCode
                                    ) jobs
                                    INNER JOIN
                                    (
                                        SELECT SUM(amount) as AmountSpent, d.DivisionCode, month(p.createddate) as Month
                                        FROM Payments p
                                        INNER JOIN Communities c
                                        ON left(p.CommunityNumber, 4) = c.CommunityNumber
                                        INNER JOIN Cities cy
                                        ON c.CityId = cy.CityId
                                        INNER join Divisions d
                                        ON c.DivisionId = d.DivisionId
                                        WHERE  cy.CityCode IN ({0}) AND
                                        YEAR(p.CreatedDate) = YEAR(getdate())
                                        GROUP by d.DivisionCode, cy.CityCode, month(p.createddate)
                                    ) payments
                                    ON jobs.DivisionCode = payments.DivisionCode";


            var markets = user.Markets;
            var result = _database.Fetch<SpentDto>(string.Format(sql, markets.CommaSeparateWrapWithSingleQuote()));
            var divisions = result.Select(x => x.DivisionCode).Distinct();

            var resultWithMonths = from month in Month.GetAll().Where(x => GetCategories().Contains(x.Abbreviation))
                                   join r in result on month.Value equals r.Month into rm
                                   from resultmonth in rm.DefaultIfEmpty(null)
                                   select
                                       new SpentDto
                                           {
                                               Amount = resultmonth == null ? 0 : resultmonth.Amount,
                                               DivisionCode = resultmonth == null ? "" : resultmonth.DivisionCode,
                                               Month = month.Value
                                           };

            var series = divisions.Select(x => new AmountSpentWidgetModel.Series
                                                {
                                                    Data = resultWithMonths.Where(r => r.DivisionCode == x || r.DivisionCode == "").Select(dto => dto.Amount).ToList(),
                                                    Name = x,
                                                });
            return series.ToList();
        }

        internal class SpentDto
        {
            public decimal Amount { get; set; }
            public string DivisionCode { get; set; }
            public int Month { get; set; }
        }
    }
}
