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

        private const string AgregationsTemplate = @"SELECT (SELECT sum(p.Amount) as Amount FROM [ServiceCalls] wc 
                                        inner join Jobs j 
                                            on wc.JobId = j.JobId   
                                        INNER JOIN Communities cm 
                                            ON j.CommunityId = cm.CommunityId 
                                        INNER JOIN Cities ci
                                            ON cm.CityId = ci.CityId 
                                        INNER JOIN Payments p
                                            ON p.JobNumber = j.JobNumber
                                        INNER JOIN Divisions d
                                            ON d.DivisionCode = ci.CityCode
                                        where {0}) / (select count(*) from Jobs j 
                                        INNER JOIN Communities cm 
                                            ON j.CommunityId = cm.CommunityId 
                                        INNER JOIN Cities ci
                                            ON cm.CityId = ci.CityId 
                                        INNER JOIN Payments p
                                            ON p.JobNumber = j.JobNumber
                                        INNER JOIN Divisions d
                                            ON d.DivisionCode = ci.CityCode
                                        where d.DivisionCode in ('HOU')
                                        and p.CreatedDate >= DATEADD(MONTH, -6, GETDATE()) and p.createddate <= getdate() 
                                        and {1})";

        private decimal GetYearToDateAmount(IUser user)
        {
            var markets = user.Markets;

            string format = string.Format(AgregationsTemplate, @"d.DivisionCode in (" + markets.CommaSeparateWrapWithSingleQuote() + ")" + "and p.CreatedDate >= DATEADD(MONTH, -6, GETDATE()) and p.createddate <= getdate() AND year(p.CreatedDate) = year(getdate())", "year(p.CreatedDate) = year(getdate())");
            var result =
                _database.Single<decimal?>(format);
            return result ?? 0;
        }

        private decimal GetQuarterToDateAmount(IUser user)
        {
            var markets = user.Markets;

            var result =
                _database.Single<decimal?>(string.Format(AgregationsTemplate, @"d.DivisionCode in (" + markets.CommaSeparateWrapWithSingleQuote() + ")" +
                                        "and p.CreatedDate >= DATEADD(MONTH, -6, GETDATE()) and p.createddate <= getdate() AND  DATEPART(qq, p.CreatedDate) = DATEPART(qq, getdate()) and year(p.CreatedDate) = year(getdate()) ", "DATEPART(qq, p.CreatedDate) = DATEPART(qq, getdate()) and year(p.CreatedDate) = year(getdate())"));
            return result ?? 0;
        }

        private decimal GetMonthToDateAmount(IUser user)
        {
            var markets = user.Markets;

            var result =
                _database.Single<decimal?>(string.Format(AgregationsTemplate, @"d.DivisionCode in (" + markets.CommaSeparateWrapWithSingleQuote() + ")" +
                                        "and p.CreatedDate >= DATEADD(MONTH, -6, GETDATE()) and p.createddate <= getdate() AND  MONTH(p.CreatedDate) = Month(getdate()) and year(p.CreatedDate) = year(getdate())", "MONTH(p.CreatedDate) = Month(getdate()) and year(p.CreatedDate) = year(getdate())"));
            return result ?? 0;
        }

        private List<AmountSpentWidgetModel.Series> GetAmountsForLastSixMonths(IUser user)
        {
            var markets = user.Markets;
            var listSeries = new List<AmountSpentWidgetModel.Series>();

                var sql = @"SELECT   sum(p.Amount) as Amount FROM [ServiceCalls] wc 
                        inner join Jobs j 
                            on wc.JobId = j.JobId   
                        INNER JOIN Communities cm 
                            ON j.CommunityId = cm.CommunityId 
                        INNER JOIN Cities ci
                            ON cm.CityId = ci.CityId 
                        INNER JOIN Payments p
                            ON p.JobNumber = j.JobNumber
                        INNER JOIN Divisions d
                            ON d.DivisionCode = ci.CityCode
                        where d.DivisionCode in (" + markets.CommaSeparateWrapWithSingleQuote() + ")" +
                        "and p.CreatedDate >= DATEADD(MONTH, -6, GETDATE()) and p.createddate <= getdate() group by month(p.CreatedDate) order by month(p.CreatedDate)";

                var result = _database.Fetch<decimal>(sql);
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

            var sql = @"SELECT DISTINCT DATENAME( month , p.CreatedDate) AS MonthName , 
                DATEPART( month , p.CreatedDate) as MonthNumber 
                      FROM
                        ServiceCalls wc 
                        INNER JOIN Jobs j
                            ON wc.JobId =  j.JobId  
                            INNER JOIN Communities cm
                        ON j.CommunityId =  cm.CommunityId
                            INNER JOIN Cities ci
                        ON cm.CityId =  ci.CityId 
                            INNER JOIN Payments p
                        ON p.JobNumber =  j.JobNumber 
                            INNER JOIN Divisions d
                        ON d.DivisionCode =  ci.CityCode
                      WHERE d.DivisionCode in (" + markets.CommaSeparateWrapWithSingleQuote() + ")" +
                        "AND p.CreatedDate >=  DATEADD( MONTH , -6 , GETDATE()) " +
                      "AND p.createddate <= GETDATE() " +
                      "ORDER BY DATEPART( month , p.CreatedDate)";
            var result = _database.Fetch<CategorieResult>(sql);
            return result.Select(x => x.MonthName.Truncate(3)).ToArray();
        }

        public class CategorieResult
        {
            public string MonthName { get; set; }
            public int MonthNumber { get; set; }
        }
    }
}
