namespace Warranty.Core.Features.PercentSurveyRecommendWidget
{
    using System;
    using System.Collections.Generic;
    using Configurations;
    using Extensions;
    using NPoco;
    using Security;
    using Services;
    using Survey.Client;
    using System.Linq;

    public class PercentSurveyRecommendWidgetQueryHandler : IQueryHandler<PercentSurveyRecommendWidgetQuery, PercentSurveyRecommendWidgetModel>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;
        private readonly ISurveyClient _surveyClient;

        public PercentSurveyRecommendWidgetQueryHandler(IDatabase database, IUserSession userSession, ISurveyClient surveyClient)
        {
            _database = database;
            _userSession = userSession;
            _surveyClient = surveyClient;
        }

        public PercentSurveyRecommendWidgetModel Handle(PercentSurveyRecommendWidgetQuery query)
        {
            var employees = GetEmployeesInMarket();

            var thisMonthRawSurveys = _surveyClient.Get.ElevenMonthWarrantySurvey(new { StartDate = SystemTime.Today.ToFirstDay(), EndDate = SystemTime.Today.ToLastDay(), EmployeeIds = employees });
            List<ApiResult> thisMonthSurveysInMarket = thisMonthRawSurveys.Details.ToObject<List<ApiResult>>();
            
            var totalThisMonthSurveys = thisMonthSurveysInMarket.Count();
            var totalThisMonthSurveysWithRecommend = thisMonthSurveysInMarket.Count(x => string.Equals(x.DefinitelyWillRecommend, SurveyConstants.DefinitelyWillThreshold, StringComparison.CurrentCultureIgnoreCase));

            var lastMonthRawSurveys = _surveyClient.Get.ElevenMonthWarrantySurvey(new { StartDate = SystemTime.Today.AddMonths(-1).ToFirstDay(), EndDate = SystemTime.Today.AddMonths(-1).ToLastDay(), EmployeeIds = employees });
            List<ApiResult> lastMonthSurveysInMarket = lastMonthRawSurveys.Details.ToObject<List<ApiResult>>();

            var totalLastMonthSurveys = lastMonthSurveysInMarket.Count();
            var totalLastMonthSurveysWithRecommend = lastMonthSurveysInMarket.Count(x => string.Equals(x.DefinitelyWillRecommend, SurveyConstants.DefinitelyWillThreshold, StringComparison.CurrentCultureIgnoreCase));

            return new PercentSurveyRecommendWidgetModel
            {
                PercentRecommendLastMonth = ServiceCallCalculator.CalculatePercentage(totalLastMonthSurveysWithRecommend, totalLastMonthSurveys),
                TotalSurveysLastMonth = totalLastMonthSurveys,
                PercentRecommendThisMonth = ServiceCallCalculator.CalculatePercentage(totalThisMonthSurveysWithRecommend, totalThisMonthSurveys),
                TotalSurveysThisMonth = totalThisMonthSurveys,
            };
        }

        private string[] GetEmployeesInMarket()
        {
            var user = _userSession.GetCurrentUser();

            using (_database)
            {
                var sql = @"SELECT DISTINCT EmployeeNumber
                            FROM Employees e
                            INNER JOIN CommunityAssignments ca
                            ON e.EmployeeId = ca.EmployeeId
                            INNER JOIN Communities c
                            ON ca.CommunityId = c.CommunityId
                            INNER JOIN Cities ci
                            ON c.CityId = ci.CityId
                            WHERE CityCode IN ({0})";

                var employeesInMarket = _database.Fetch<string>(string.Format(sql, user.Markets.CommaSeparateWrapWithSingleQuote()));
                return employeesInMarket.ToArray();
            }
        } 

        internal class ApiResult
        {
            public string WarrantyServiceRepresentativeEmployeeId { get; set; }
            public string DefinitelyWillRecommend { get; set; }
        }
    }
}