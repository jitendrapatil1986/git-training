namespace Warranty.Core.Features.Report.WarrantyBonusSummary
{
    using System.Collections.Generic;
    using Extensions;
    using NPoco;
    using Security;

    public class WarrantyBonusSummaryQueryHandler : IQueryHandler<WarrantyBonusSummaryQuery, WarrantyBonusSummaryModel>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public WarrantyBonusSummaryQueryHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public WarrantyBonusSummaryModel Handle(WarrantyBonusSummaryQuery message)
        {
            var model = new WarrantyBonusSummaryModel
                {
                    BonusSummaries = GetBonusByCommunity(),
                };

            return model;
        }

        private IEnumerable<WarrantyBonusSummaryModel.BonusSummary> GetBonusByCommunity()
        {
            var user = _userSession.GetCurrentUser();

            using (_database)
            {
                const string sql = @"SELECT c.[CommunityName], b.NumberOfWarrantableHomes, a.*
                            FROM
                            (
                                SELECT  c.[CommunityId],
                                [ObjectAccount],
                                SUM(Amount) as TotalDollarsSpent,
                                SUM(CASE WHEN [ObjectAccount] = '9425' THEN Amount ELSE 0 END) as MaterialDollarsSpent,
                                SUM(CASE WHEN [ObjectAccount] = '9430' THEN Amount ELSE 0 END) as LaborDollarsSpent,
                                SUM(CASE WHEN [ObjectAccount] = '9435' THEN Amount ELSE 0 END) as OtherMaterialDollarsSpent,
                                SUM(CASE WHEN [ObjectAccount] = '9440' THEN Amount ELSE 0 END) as OtherLaborDollarsSpent
                                FROM WarrantyPayments p
                                INNER JOIN Jobs j
                                ON p.JobNumber = j.JobNumber
                                INNER JOIN Communities c
                                ON j.CommunityId = c.CommunityId
                                INNER JOIN Cities cc
                                ON c.CityId = cc.CityId
                                INNER JOIN CommunityAssignments ca
                                ON c.CommunityId = ca.CommunityId
                                WHERE PostingMonth = MONTH(@1) AND PostingYear = YEAR(@1)
                                GROUP BY c.[CommunityId], [ObjectAccount]
                            ) a
                            INNER JOIN Communities c
                            ON c.CommunityId = a.CommunityId
                            INNER JOIN
                            (
                                SELECT COUNT(*) as NumberOfWarrantableHomes, j.CommunityId
                                FROM Jobs j
                                INNER JOIN Communities c
                                ON j.CommunityId = c.CommunityId
                                INNER JOIN Cities Ci
                                ON c.CityId = Ci.CityId
                                INNER JOIN CommunityAssignments ca
                                ON c.CommunityId = ca.CommunityId
                                WHERE CloseDate >= DATEADD(yy, @0, @1)
                                AND CloseDate <= @1
                                AND Ci.CityCode IN ({0})
                                GROUP BY j.CommunityId
                            ) b 
                            ON a.[CommunityId] = b.[CommunityId]
                            ORDER BY c.CommunityName";

                var result = _database.Fetch<WarrantyBonusSummaryModel.BonusSummary>(string.Format(sql, user.Markets.CommaSeparateWrapWithSingleQuote()), -2, SystemTime.Today, user.EmployeeNumber);

                return result;
            }
        }
    }
}