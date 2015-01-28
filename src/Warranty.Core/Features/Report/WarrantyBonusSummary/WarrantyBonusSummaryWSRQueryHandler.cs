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
                    const string sql = @"SELECT d.DivisionName, e.EmployeeName, e.EmployeeNumber, c.[CommunityName], b.NumberOfWarrantableHomes, a.*,
                                            (b.NumberofWarrantableHomes * @3) as TotalWarrantyAllowance,
                                            ((b.NumberOfWarrantableHomes * @3) - a.TotalDollarsSpent) as TotalWarrantyDifference
                                        FROM
                                        (
                                            --Payments made in Warranty system tied to Service Call Line item.
                                            SELECT  c.[CommunityId],
                                                e.[EmployeeId],
                                                SUM(Amount) as TotalDollarsSpent,
                                                SUM(CASE WHEN [ObjectAccount] = '9425' THEN Amount ELSE 0 END) as MaterialDollarsSpent,
                                                SUM(CASE WHEN [ObjectAccount] = '9430' THEN Amount ELSE 0 END) as LaborDollarsSpent,
                                                SUM(CASE WHEN [ObjectAccount] = '9435' THEN Amount ELSE 0 END) as OtherMaterialDollarsSpent,
                                                SUM(CASE WHEN [ObjectAccount] = '9440' THEN Amount ELSE 0 END) as OtherLaborDollarsSpent
                                            FROM Payments p
                                            INNER JOIN ServiceCallLineItems li
                                            ON li.ServiceCallLineItemId = p.ServiceCallLineItemId
                                            INNER JOIN ServiceCalls sc
                                            ON sc.ServiceCallId = li.ServiceCallId
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
                                                AND EmployeeNumber = @2
                                            WHERE 
                                            cc.CityCode = @4
                                            AND p.ServiceCallLineItemId IS NOT NULL
                                            AND MONTH(sc.CreatedDate) = MONTH(@1) 
                                            AND YEAR(sc.CreatedDate) = YEAR(@1)
                                            AND sc.CreatedDate >= j.CloseDate
                                            AND sc.CreatedDate <= DATEADD(yy, 2, j.CloseDate)
                                            AND p.PaymentStatus = @5
                                            AND sc.SpecialProject = 0
                                            GROUP BY c.[CommunityId], e.[EmployeeId]

                                            UNION

                                            --Payments made outside Warranty system not tied to Service Call Line item.
                                            SELECT  c.[CommunityId],
                                                    e.[EmployeeId],
                                                    SUM(Amount) as TotalDollarsSpent,
                                                    SUM(CASE WHEN [ObjectAccount] = '9425' THEN Amount ELSE 0 END) as MaterialDollarsSpent,
                                                    SUM(CASE WHEN [ObjectAccount] = '9430' THEN Amount ELSE 0 END) as LaborDollarsSpent,
                                                    SUM(CASE WHEN [ObjectAccount] = '9435' THEN Amount ELSE 0 END) as OtherMaterialDollarsSpent,
                                                    SUM(CASE WHEN [ObjectAccount] = '9440' THEN Amount ELSE 0 END) as OtherLaborDollarsSpent
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
                                                AND EmployeeNumber = @2
                                            WHERE
                                            cc.CityCode = @4
                                            AND p.ServiceCallLineItemId IS NULL
                                            AND MONTH(p.CreatedDate) = MONTH(@1) 
                                            AND YEAR(p.CreatedDate) = YEAR(@1)
                                            AND p.CreatedDate >= j.CloseDate
                                            AND p.CreatedDate <= DATEADD(yy, @0, j.CloseDate)
                                            AND p.PaymentStatus = @5
                                            GROUP BY c.[CommunityId], e.[EmployeeId]
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
                                            INNER JOIN Employees e
                                            ON ca.EmployeeId = e.EmployeeId
                                            WHERE CloseDate >= DATEADD(yy, -@0, @1)
                                            AND CloseDate <= @1
                                            AND Ci.CityCode = @4
                                            AND EmployeeNumber=@2
                                            GROUP BY j.CommunityId
                                        ) b
                                        ON a.[CommunityId] = b.[CommunityId]
                                        INNER JOIN Divisions d
                                        ON c.DivisionId = d.DivisionId
                                        INNER JOIN Employees e
                                        ON a.EmployeeId = e.EmployeeId
                                        ORDER BY c.CommunityName";

                    result = _database.Fetch<WarrantyBonusSummaryModel.BonusSummary>(sql, 2, query.Model.StartDate, employeeNumber, dollarsSpent, market, PaymentStatus.Paid.Value);
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