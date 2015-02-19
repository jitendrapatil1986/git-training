namespace Warranty.Core.Features.Report.WarrantyBonusSummary
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configurations;
    using Enumerations;
    using Extensions;
    using NPoco;
    using Security;
    using Survey.Client;

    public class WarrantyBonusSummaryWSRQueryHandler : IQueryHandler<WarrantyBonusSummaryWSRQuery, WarrantyBonusSummaryModel>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;
        private readonly ISurveyClient _survey;

        public WarrantyBonusSummaryWSRQueryHandler(IDatabase database, IUserSession userSession, ISurveyClient survey)
        {
            _database = database;
            _userSession = userSession;
            _survey = survey;
        }

        public WarrantyBonusSummaryModel Handle(WarrantyBonusSummaryWSRQuery query)
        {
            var user = _userSession.GetCurrentUser();
            var employeeNumber = user.IsInRole(UserRoles.WarrantyServiceRepresentative) ? user.EmployeeNumber : query.Model.SelectedEmployeeNumber;
            var market = user.Markets.FirstOrDefault();

            if (!query.Model.FilteredDate.HasValue || String.IsNullOrEmpty(market))
            {
                return new WarrantyBonusSummaryModel
                {
                    EmployeeTiedToRepresentatives = GetEmployeesTiedToRepresentatives(user),
                };
            }

            
            var surveyData = GetSurveyData(query, employeeNumber);

            var model = new WarrantyBonusSummaryModel
                {
                    BonusSummaries = GetBonusByEmployeeAndCommunity(query, employeeNumber, market),
                    EmployeeTiedToRepresentatives = GetEmployeesTiedToRepresentatives(user),
                    DefinitelyWouldRecommendSurveys = surveyData.Select(x=> new WarrantyBonusSummaryModel.DefinitelyWouldRecommendSurvey{DefinitelyWillRecommend = x.DefinitelyWillRecommend, HomeownerName = x.HomeownerName, JobNumber = x.JobNumber}),
                    ExcellentWarrantySurveys = surveyData.Select(x => new WarrantyBonusSummaryModel.ExcellentWarrantySurvey{ExcellentWarrantyService = x.ExcellentWarrantyService, HomeownerName = x.HomeownerName, JobNumber = x.JobNumber}),
                    AllItemsCompletes = GetAllItemsComplete(query, employeeNumber),
                };

            model.SelectedEmployeeNumber = employeeNumber;
            model.FilteredDate = query.Model != null ? query.Model.FilteredDate : null;

            return model;
        }

        private IEnumerable<WarrantyBonusSummaryModel.BonusSummary> GetBonusByEmployeeAndCommunity(WarrantyBonusSummaryWSRQuery query, string employeeNumber, string market)
        {
            var result = new List<WarrantyBonusSummaryModel.BonusSummary>();

            if (!string.IsNullOrEmpty(employeeNumber))
            {
                var dollarsSpent = WarrantyConfigSection.GetCity(market).WarrantyAmount;

                using (_database)
                {
                    const string sql = @"; WITH AllWarrantyPayments (FiscalYear, Month, Year, CostCenter, ObjectAccount, Amount, CommunityNumber, FirstDayOfMonth, LastDayOfMonth)
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

                                            SELECT d.DivisionName, e.EmployeeName, e.EmployeeNumber, a.CommunityId, c.CommunityName, a.NumberOfWarrantableHomes, e.EmployeeId, ISNULL(b.TotalDollarsSpent, 0) as TotalDollarsSpent,
                                                ISNULL(b.MaterialDollarsSpent, 0) as MaterialDollarsSpent, ISNULL(b.LaborDollarsSpent, 0) as LaborDollarsSpent, ISNULL(b.OtherMaterialDollarsSpent, 0) as OtherMaterialDollarsSpent, 
                                                ISNULL(b.OtherLaborDollarsSpent, 0) as OtherLaborDollarsSpent,
                                                ISNULL((a.NumberofWarrantableHomes * @4), 0) as TotalWarrantyAllowance,
                                                (ISNULL((a.NumberOfWarrantableHomes * @4), 0) - ISNULL(b.TotalDollarsSpent, 0)) as TotalWarrantyDifference
                                            FROM
                                            (
                                                SELECT COUNT(*) as NumberOfWarrantableHomes, j.CommunityId, e.EmployeeId
                                                FROM Jobs j
                                                INNER JOIN Communities c
                                                ON j.CommunityId = c.CommunityId
                                                INNER JOIN Cities Ci
                                                ON c.CityId = Ci.CityId
                                                INNER JOIN CommunityAssignments ca
                                                ON c.CommunityId = ca.CommunityId
                                                INNER JOIN Employees e
                                                ON ca.EmployeeId = e.EmployeeId
                                                WHERE CloseDate >= DATEADD(yy, -@0, @1)
                                                AND CloseDate <= @2
                                                AND Ci.CityCode = @5
                                                AND EmployeeNumber=@3
                                                GROUP BY j.CommunityId, e.EmployeeId
                                            ) a
                                            INNER JOIN Communities c
                                            ON c.CommunityId = a.CommunityId
                                            LEFT JOIN
                                            (
                                                SELECT [CommunityId],
                                                    [EmployeeId],
                                                    SUM(Amount) as TotalDollarsSpent,
                                                    SUM(CASE WHEN [ObjectAccount] = '9425' THEN Amount ELSE 0 END) as MaterialDollarsSpent,
                                                    SUM(CASE WHEN [ObjectAccount] = '9430' THEN Amount ELSE 0 END) as LaborDollarsSpent,
                                                    SUM(CASE WHEN [ObjectAccount] = '9435' THEN Amount ELSE 0 END) as OtherMaterialDollarsSpent,
                                                    SUM(CASE WHEN [ObjectAccount] = '9440' THEN Amount ELSE 0 END) as OtherLaborDollarsSpent
                                                FROM 
                                                (
                                                    SELECT DISTINCT p.*, c.CommunityId, e.EmployeeId FROM AllWarrantyPayments p
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
                                                        AND EmployeeNumber=@3
                                                    WHERE
                                                        cc.CityCode = @5
                                                        AND p.FirstDayOfMonth >= @1
                                                        AND p.LastDayOfMonth <= @2
                                                        AND p.LastDayOfMonth <= DATEADD(yy, @0, j.CloseDate)
                                                ) payments
                                                GROUP BY [CommunityId], [EmployeeId]
                                            ) b
                                            ON a.[CommunityId] = b.[CommunityId]
                                            INNER JOIN Divisions d
                                            ON c.DivisionId = d.DivisionId
                                            INNER JOIN Employees e
                                            ON a.EmployeeId = e.EmployeeId
                                            ORDER BY c.CommunityName";

                    result = _database.Fetch<WarrantyBonusSummaryModel.BonusSummary>(sql, 2, query.Model.StartDate, query.Model.EndDate, employeeNumber, dollarsSpent, market);
                }
            }

            return result;
        }

        private IEnumerable<WarrantyBonusSummaryModel.EmployeeTiedToRepresentative> GetEmployeesTiedToRepresentatives(IUser user)
        {
            var result = new List<WarrantyBonusSummaryModel.EmployeeTiedToRepresentative>();

            if (user.IsInRole(UserRoles.CustomerCareManager) || user.IsInRole(UserRoles.WarrantyServiceCoordinator) || user.IsInRole(UserRoles.WarrantyAdmin))
            {
                const string sql = @"SELECT DISTINCT e.EmployeeId as WarrantyRepresentativeEmployeeId, e.EmployeeNumber, LOWER(e.EmployeeName) as EmployeeName from CommunityAssignments ca
                                    INNER join Communities c
                                    ON ca.CommunityId = c.CommunityId
                                    INNER join Employees e
                                    ON ca.EmployeeId = e.EmployeeId
                                    INNER JOIN Cities ci
                                    ON c.CityId = ci.CityId
                                    WHERE CityCode IN ({0})
                                    ORDER BY EmployeeName";

                using (_database)
                {
                    result = _database.Fetch<WarrantyBonusSummaryModel.EmployeeTiedToRepresentative>(string.Format(sql, user.Markets.CommaSeparateWrapWithSingleQuote()));
                }
            }

            return result;
        }

        private IEnumerable<SurveyDataResult> GetSurveyData(WarrantyBonusSummaryWSRQuery query, string employeeNumber)
        {
            var surveyData = _survey.Get.ElevenMonthWarrantySurvey(new { query.Model.StartDate, query.Model.EndDate, EmployeeId = employeeNumber });
            return surveyData.Details.ToObject<List<SurveyDataResult>>();
        }

        private IEnumerable<WarrantyBonusSummaryModel.ItemsComplete> GetAllItemsComplete(WarrantyBonusSummaryWSRQuery query, string employeeNumber)
        {
            var surveyData = _survey.Get.OneMonthWarrantySurvey(new { query.Model.StartDate, query.Model.EndDate, EmployeeId = employeeNumber });

            List<ItemsCompleteResult> itemsCompleteResults = surveyData.Details.ToObject<List<ItemsCompleteResult>>();
            var result = itemsCompleteResults.GroupBy(x => x.CommunityName)
                                             .Select(g => new WarrantyBonusSummaryModel.ItemsComplete
                                             {
                                                 CommunityName = g.Key,
                                                 CompletePercentage = g.Average(y => string.Equals(y.ItemsCompleted, SurveyConstants.AllItemsCompleteThreshold, StringComparison.CurrentCultureIgnoreCase) ? 100m : 0m),
                                             }).OrderBy(o => o.CommunityName).ToList();
            return result;
        }

        internal class ItemsCompleteResult
        {
            public string CommunityName { get; set; }
            public string JobNumber { get; set; }
            public string ItemsCompleted { get; set; }
        }

        internal class SurveyDataResult
        {
            public string DefinitelyWillRecommend { get; set; }
            public string HomeownerName { get; set; }
            public string JobNumber { get; set; }
            public string ExcellentWarrantyService { get; set; }
        }
    }
}