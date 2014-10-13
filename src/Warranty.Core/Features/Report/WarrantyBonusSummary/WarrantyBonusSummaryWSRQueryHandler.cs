namespace Warranty.Core.Features.Report.WarrantyBonusSummary
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
        private const decimal costControlBonusPercent = (decimal) 0.10;
        private const decimal allItemsCompletePercentThreshold = 85;
        private const decimal allItemsCompleteBonusAmount = 100;
        private const string definitelyWillThreshold = "DEFINITELY WILL";
        private const decimal definitelyWillBonusAmount = 50;
        private const int excellentWarrantyThreshold = 9;
        private const decimal excellentWarrantyBonusAmount = 50;

        public WarrantyBonusSummaryWSRQueryHandler(IDatabase database, IUserSession userSession, ISurveyClient survey)
        {
            _database = database;
            _userSession = userSession;
            _survey = survey;
        }

        public WarrantyBonusSummaryModel Handle(WarrantyBonusSummaryWSRQuery query)
        {
            var model = new WarrantyBonusSummaryModel
                {
                    BonusSummaries = GetBonusByEmployeeAndCommunity(query),
                    EmployeeTiedToRepresentatives = GetEmployeesTiedToRepresentatives(),
                    DefinitelyWouldRecommendSurveys = GetDefinitelyWouldRecommendSurveys(query),
                    ExcellentWarrantySurveys = GetExcellentWarrantySurveys(query),
                    AllItemsCompletes = GetAllItemsComplete(),
                    MiscellaneousBonuses = GetMiscellaneousBonuses(),
                };

            foreach (var bonusSummary in model.BonusSummaries)
            {
                model.TotalNumberOfWarrantableHomes += bonusSummary.NumberOfWarrantableHomes;
                model.TotalMaterialDollarsSpent += bonusSummary.MaterialDollarsSpent;
                model.TotalLaborDollarsSpent += bonusSummary.LaborDollarsSpent;
                model.TotalOtherMaterialDollarsSpent += bonusSummary.OtherMaterialDollarsSpent;
                model.TotalOtherLaborDollarsSpent += bonusSummary.OtherLaborDollarsSpent;
                model.TotalDollarsSpent += bonusSummary.TotalDollarsSpent;
                model.TotalWarrantyAllowance += bonusSummary.TotalWarrantyAllowance;
                model.TotalWarrantyDifference += bonusSummary.TotalWarrantyDifference;
            }

            model.TotalCostControlBonusAmount = model.TotalWarrantyDifference * costControlBonusPercent;

            foreach (var definitelyWouldRecommendSurvey in model.DefinitelyWouldRecommendSurveys)
            {
                if (!string.IsNullOrEmpty(definitelyWouldRecommendSurvey.DefinitelyWillRecommend))
                {
                    if (definitelyWouldRecommendSurvey.DefinitelyWillRecommend.ToUpper() == definitelyWillThreshold)
                    {
                        definitelyWouldRecommendSurvey.BonusAmount = definitelyWillBonusAmount;
                        model.TotalDefinitelyWouldRecommendSurveyBonusAmount += definitelyWillBonusAmount;
                    }
                }
            }

            foreach (var excellentWarrantySurvey in model.ExcellentWarrantySurveys)
            {
                if (!string.IsNullOrEmpty(excellentWarrantySurvey.ExcellentWarrantyService))
                {
                    if (Convert.ToInt16(excellentWarrantySurvey.ExcellentWarrantyService) >= excellentWarrantyThreshold)
                    {
                        excellentWarrantySurvey.BonusAmount = excellentWarrantyBonusAmount;
                        model.TotalExcellentWarrantySurveyBonusAmount += excellentWarrantyBonusAmount;
                    }
                }
            }

            foreach (var miscellaneousBonuse in model.MiscellaneousBonuses)
            {
                model.TotalMiscellaneousBonusAmount += miscellaneousBonuse.BonusAmount;
            }

            if (model.AllItemsCompletes.Any())
            {
                var totalPercentComplete = model.AllItemsCompletes.Sum(allItemsComplete => allItemsComplete.CompletePercentage);
                var averagePercentComplete = totalPercentComplete/model.AllItemsCompletes.Count();
                model.TotalAllItemsCompleteBonusAmount = averagePercentComplete >= allItemsCompletePercentThreshold ? allItemsCompleteBonusAmount : 0;
            }

            model.TotalRepresentativeBonusAmount = (model.TotalCostControlBonusAmount > 0 ? model.TotalCostControlBonusAmount : 0) + model.TotalDefinitelyWouldRecommendSurveyBonusAmount + model.TotalExcellentWarrantySurveyBonusAmount +
                                                   model.TotalMiscellaneousBonusAmount + model.TotalAllItemsCompleteBonusAmount;

            model.AnyResults = (model.BonusSummaries.Any() || model.DefinitelyWouldRecommendSurveys.Any() || model.ExcellentWarrantySurveys.Any() || model.AllItemsCompletes.Any() || model.MiscellaneousBonuses.Any());

            if (query.queryModel != null)
            {
                model.FilteredDate = query.queryModel.FilteredDate;
            }

            return model;
        }

        private IEnumerable<WarrantyBonusSummaryModel.BonusSummary> GetBonusByEmployeeAndCommunity(WarrantyBonusSummaryWSRQuery message)
        {
            var user = _userSession.GetCurrentUser();

            var employeeNumber = user.EmployeeNumber;

            if (!user.IsInRole(UserRoles.WarrantyServiceRepresentative))
            {
                if (message.queryModel != null)
                {
                    if (!string.IsNullOrEmpty(message.queryModel.SelectedEmployeeNumber))
                    {
                        employeeNumber = message.queryModel.SelectedEmployeeNumber;
                    }
                }
            }

            var startDate = SystemTime.Today;

            if (message.queryModel != null)
            {
                if (message.queryModel.FilteredDate != null)
                {
                    startDate = message.queryModel.FilteredDate.Value.AddDays(1 - message.queryModel.FilteredDate.Value.Day);
                }
            }

            using (_database)
            {
                const string sql = @"SELECT d.DivisionName, e.EmployeeName, e.EmployeeNumber, c.[CommunityName], b.NumberOfWarrantableHomes, a.*, 
                                    (b.NumberofWarrantableHomes * 40) as TotalWarrantyAllowance, ((b.NumberOfWarrantableHomes * 40) - a.TotalDollarsSpent) as TotalWarrantyDifference
                                    FROM
                                    (
                                        SELECT  c.[CommunityId],
                                        e.[EmployeeId],
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
                                        INNER JOIN Employees e
                                        ON ca.EmployeeId = e.EmployeeId
                                        WHERE PostingMonth = MONTH(@1) AND PostingYear = YEAR(@1) AND EmployeeNumber=@2
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
                                        WHERE CloseDate >= DATEADD(yy, @0, @1)
                                        AND CloseDate <= @1
                                        AND Ci.CityCode IN ({0})
                                        AND EmployeeNumber=@2
                                        GROUP BY j.CommunityId
                                    ) b
                                    ON a.[CommunityId] = b.[CommunityId]
                                    INNER JOIN Divisions d
                                    ON c.DivisionId = d.DivisionId
                                    INNER JOIN Employees e
                                    ON a.EmployeeId = e.EmployeeId
                                    ORDER BY c.CommunityName";

                var result = _database.Fetch<WarrantyBonusSummaryModel.BonusSummary>(string.Format(sql, user.Markets.CommaSeparateWrapWithSingleQuote()), -2, startDate, employeeNumber);

                return result;
            }
        }

        private IEnumerable<WarrantyBonusSummaryModel.EmployeeTiedToRepresentative> GetEmployeesTiedToRepresentatives()
        {
            var user = _userSession.GetCurrentUser();
            
            const string sql = @"SELECT e.EmployeeNumber, a.* FROM Employees e
                                INNER JOIN
                                (
                                    SELECT WarrantyRepresentativeEmployeeId
                                        , LOWER(e.EmployeeName) as EmployeeName
                                    FROM [ServiceCalls] sc
                                    INNER JOIN Employees e
                                    ON sc.WarrantyRepresentativeEmployeeId = e.EmployeeId
                                    INNER JOIN Jobs j
                                    ON sc.JobId = j.JobId
                                    INNER JOIN Communities cm
                                    ON j.CommunityId = cm.CommunityId
                                    INNER JOIN Cities ci
                                    ON cm.CityId = ci.CityId
                                    WHERE CityCode IN ({0})
                                    AND EmployeeNumber <> ''
                                    GROUP BY WarrantyRepresentativeEmployeeId, e.EmployeeName
                                ) a
                                ON e.EmployeeId = a.WarrantyRepresentativeEmployeeId
                                {1} /*WHERE */
                                ORDER BY e.EmployeeName";

            var additionalWhereClause = "";

            if (user.IsInRole(UserRoles.WarrantyServiceRepresentative))
            {
                additionalWhereClause += "AND EmployeeNumber = " + user.EmployeeNumber + "";
            }

            using (_database)
            {
                var result = _database.Fetch<WarrantyBonusSummaryModel.EmployeeTiedToRepresentative>(string.Format(sql, user.Markets.CommaSeparateWrapWithSingleQuote(), additionalWhereClause));

                return result;
            }
        }

        private IEnumerable<WarrantyBonusSummaryModel.MiscellaneousBonus> GetMiscellaneousBonuses()
        {
            using (_database)
            {
                const string sql = @"SELECT 'Test Misc' as Description, 35 as BonusAmount WHERE 1 = 0";  //TODO: Replace with real script.

                var result = _database.Fetch<WarrantyBonusSummaryModel.MiscellaneousBonus>(sql);

                return result;
            }
        }

        private IEnumerable<WarrantyBonusSummaryModel.ItemsComplete> GetAllItemsComplete()
        {
            using (_database)
            {
                const string sql = @"SELECT 'Test Community' as CommunityName, 85 as CompletePercentage WHERE 1 = 0";  //TODO: Replace with real script.

                var result = _database.Fetch<WarrantyBonusSummaryModel.ItemsComplete>(sql);

                return result;
            }
        }

        private IEnumerable<WarrantyBonusSummaryModel.ExcellentWarrantySurvey> GetExcellentWarrantySurveys(WarrantyBonusSummaryWSRQuery query)
        {
            var user = _userSession.GetCurrentUser();
            var startDate = SystemTime.Today;
            var endDate = SystemTime.Today;

            if (query.queryModel != null)
            {
                if (query.queryModel.FilteredDate != null)
                {
                    startDate = query.queryModel.FilteredDate.Value.AddDays(1 - query.queryModel.FilteredDate.Value.Day);
                    endDate = query.queryModel.FilteredDate.Value.AddMonths(1).AddDays(-query.queryModel.FilteredDate.Value.Day);
                }
            }

            var employeeNumber = user.EmployeeNumber;

            if (!user.IsInRole(UserRoles.WarrantyServiceRepresentative))
            {
                if (query.queryModel != null)
                {
                    if (!string.IsNullOrEmpty(query.queryModel.SelectedEmployeeNumber))
                    {
                        employeeNumber = query.queryModel.SelectedEmployeeNumber;
                    }
                }
            }

            var surveyData = _survey.Get.ElevenMonthWarrantySurvey(new
            {
                StartDate = startDate,
                EndDate = endDate,
            });

            IEnumerable<WarrantyBonusSummaryModel.ExcellentWarrantySurvey> results =
                surveyData.Details.ToObject<List<WarrantyBonusSummaryModel.ExcellentWarrantySurvey>>();

            results = results.Where(x => x.WarrantyServiceRepresentativeEmployeeId == employeeNumber);

            return results;
        }

        private IEnumerable<WarrantyBonusSummaryModel.DefinitelyWouldRecommendSurvey> GetDefinitelyWouldRecommendSurveys(WarrantyBonusSummaryWSRQuery query)
        {
            var user = _userSession.GetCurrentUser();
            var startDate = SystemTime.Today;
            var endDate = SystemTime.Today;

            if (query.queryModel != null)
            {
                if (query.queryModel.FilteredDate != null)
                {
                    startDate = query.queryModel.FilteredDate.Value.AddDays(1 - query.queryModel.FilteredDate.Value.Day);
                    endDate = query.queryModel.FilteredDate.Value.AddMonths(1).AddDays(-query.queryModel.FilteredDate.Value.Day);
                }
            }

            var employeeNumber = user.EmployeeNumber;

            if (!user.IsInRole(UserRoles.WarrantyServiceRepresentative))
            {
                if (query.queryModel != null)
                {
                    if (!string.IsNullOrEmpty(query.queryModel.SelectedEmployeeNumber))
                    {
                        employeeNumber = query.queryModel.SelectedEmployeeNumber;
                    }
                }
            }

            var surveyData = _survey.Get.ElevenMonthWarrantySurvey(new
                {
                    StartDate = startDate,
                    EndDate = endDate,
                });

            IEnumerable<WarrantyBonusSummaryModel.DefinitelyWouldRecommendSurvey> results =
                surveyData.Details.ToObject<List<WarrantyBonusSummaryModel.DefinitelyWouldRecommendSurvey>>();
            
            results = results.Where(x => x.WarrantyServiceRepresentativeEmployeeId == employeeNumber);

            return results;
        }

    }
}