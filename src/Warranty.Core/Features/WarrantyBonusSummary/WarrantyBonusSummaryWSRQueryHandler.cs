﻿namespace Warranty.Core.Features.WarrantyBonusSummary
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Enumerations;
    using Extensions;
    using NPoco;
    using Security;

    public class WarrantyBonusSummaryWSRQueryHandler : IQueryHandler<WarrantyBonusSummaryWSRQuery, WarrantyBonusSummaryModel>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public WarrantyBonusSummaryWSRQueryHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public WarrantyBonusSummaryModel Handle(WarrantyBonusSummaryWSRQuery message)
        {
            var model = new WarrantyBonusSummaryModel
                {
                    BonusSummaries = GetBonusByEmployeeAndCommunity(message),
                    EmployeeTiedToRepresentatives = GetEmployeesTiedToRepresentatives(),
                    DefinitelyWouldRecommendSurveys = GetDefinitelyWouldRecommendSurveys(),
                    ExcellentWarrantySurveys = GetExcellentWarrantySurveys(),
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

            foreach (var definitelyWouldRecommendSurvey in model.DefinitelyWouldRecommendSurveys)
            {
                if (definitelyWouldRecommendSurvey.DefinitelyWouldRecommend)
                {
                    model.TotalDefinitelyWouldRecommendSurveyBonusAmount += definitelyWouldRecommendSurvey.BonusAmount;
                }
            }

            foreach (var excellentWarrantySurvey in model.ExcellentWarrantySurveys)
            {
                if (excellentWarrantySurvey.ExcellentWarranty)
                {
                    model.TotalExcellentWarrantySurveyBonusAmount += excellentWarrantySurvey.BonusAmount;
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
                model.TotalAllItemsCompleteBonusAmount = averagePercentComplete >= 85 ? 100 : 0;
            }

            model.TotalRepresentativeBonusAmount = model.TotalWarrantyDifference + model.TotalDefinitelyWouldRecommendSurveyBonusAmount + model.TotalExcellentWarrantySurveyBonusAmount +
                                                   model.TotalMiscellaneousBonusAmount + model.TotalAllItemsCompleteBonusAmount;

            if (message.queryModel != null)
            {
                model.FilteredDate = message.queryModel.FilteredDate;
            }

            return model;
        }

        private IEnumerable<WarrantyBonusSummaryModel.BonusSummary> GetBonusByEmployeeAndCommunity(WarrantyBonusSummaryWSRQuery message)
        {
            var user = _userSession.GetCurrentUser();

            var employeeNumber = user.EmployeeNumber;

            //if (!(user.IsInRole(UserRoles.WarrantyServiceRepresentative) && user.IsInRole(UserRoles.WarrantyServiceManager)))
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
        --*****                                AND Ci.CityCode IN ({0})
                                        AND EmployeeNumber=@2
                                        GROUP BY j.CommunityId
                                    ) b
                                    ON a.[CommunityId] = b.[CommunityId]
                                    INNER JOIN Divisions d
                                    ON c.DivisionId = d.DivisionId
                                    INNER JOIN Employees e
                                    ON a.EmployeeId = e.EmployeeId
                                    ORDER BY c.CommunityName";

                //var result = _database.Fetch<WarrantyBonusSummaryModel.BonusSummary>(string.Format(sql, user.Markets.CommaSeparateWrapWithSingleQuote()), -2, startDate, employeeNumber);
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
--*****                                    WHERE CityCode IN ({0})
                                    GROUP BY WarrantyRepresentativeEmployeeId, e.EmployeeName
                                ) a
                                ON e.EmployeeId = a.WarrantyRepresentativeEmployeeId
                                {1} /*WHERE */
                                ORDER BY e.EmployeeName";

            var whereClause = "";

            //if ((user.IsInRole(UserRoles.WarrantyServiceRepresentative) || user.IsInRole(UserRoles.WarrantyServiceManager)))
            if (user.IsInRole(UserRoles.WarrantyServiceRepresentative))
            {
                whereClause = "WHERE EmployeeNumber = " + user.EmployeeNumber + "";
            }

            var result = _database.Fetch<WarrantyBonusSummaryModel.EmployeeTiedToRepresentative>(string.Format(sql, user.Markets.CommaSeparateWrapWithSingleQuote(), whereClause));

            return result;
        }

        private IEnumerable<WarrantyBonusSummaryModel.MiscellaneousBonus> GetMiscellaneousBonuses()
        {
            using (_database)
            {
                const string sql = @"SELECT 'Test Misc' as Description, 35 as BonusAmount";  //TODO: Replace with real script.

                var result = _database.Fetch<WarrantyBonusSummaryModel.MiscellaneousBonus>(sql);

                return result;
            }
        }

        private IEnumerable<WarrantyBonusSummaryModel.ItemsComplete> GetAllItemsComplete()
        {
            using (_database)
            {
                const string sql = @"SELECT 'Test Community' as CommunityName, 90 as CompletePercentage";  //TODO: Replace with real script.

                var result = _database.Fetch<WarrantyBonusSummaryModel.ItemsComplete>(sql);

                return result;
            }
        }

        private IEnumerable<WarrantyBonusSummaryModel.ExcellentWarrantySurvey> GetExcellentWarrantySurveys()
        {
            using (_database)
            {
                const string sql = @"SELECT 'Test Customer' as CustomerName, 1 as ExcellentWarranty";  //TODO: Replace with real script.

                var result = _database.Fetch<WarrantyBonusSummaryModel.ExcellentWarrantySurvey>(sql);

                return result;
            }
        }

        private IEnumerable<WarrantyBonusSummaryModel.DefinitelyWouldRecommendSurvey> GetDefinitelyWouldRecommendSurveys()
        {
            using (_database)
            {
                const string sql = @"SELECT 'Test Customer' as CustomerName, 1 as DefinitelyWouldRecommend";  //TODO: Replace with real script.

                var result = _database.Fetch<WarrantyBonusSummaryModel.DefinitelyWouldRecommendSurvey>(sql);

                return result;
            }
        }

    }
}