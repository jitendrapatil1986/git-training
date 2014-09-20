namespace Warranty.Core.Features.WarrantyDollarsSpentWidget
{
    using System;
    using NPoco;
    using Security;
    using Extensions;

    public class WarrantyDollarsSpentWidgetQuery : IQuery<WarrantyDollarsSpentWidgetModel>
    {
    }

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
                            AND Ci.CityCode IN ({0})";

                var sqlPayments = @"SELECT SUM(p.Amount) as TotalDollarsSpent
                                    FROM Jobs j
                                    INNER JOIN Communities c
                                    ON j.CommunityId = c.CommunityId
                                    INNER JOIN Cities Ci
                                    ON c.CityId = Ci.CityId
                                    INNER JOIN Payments p
                                    ON j.JobNumber = p.JobNumber
                                    WHERE Ci.CityCode IN ({0}) AND MONTH(p.CreatedDate) = @0 AND YEAR(p.CreatedDate) = @1";

                var numberOfHomes = _database.ExecuteScalar<int>(string.Format(sqlNumberOfHomes, user.Markets.CommaSeparateWrapWithSingleQuote()), -2, DateTime.Today);
                var totalPayments = _database.ExecuteScalar<decimal>(string.Format(sqlPayments, user.Markets.CommaSeparateWrapWithSingleQuote()), DateTime.Today.Month, DateTime.Today.Year);

                return new WarrantyDollarsSpentWidgetModel
                           {
                               AmountSpent = totalPayments/numberOfHomes,
                           };
            }
        }
    }
}