namespace Warranty.Core.Features.PercentSurveyRecommendWidget
{
    using System;
    using System.Collections.Generic;
    using Configurations;
    using Extensions;
    using NPoco;
    using Security;
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
           
            var thisMonthRawSurveys = _surveyClient.Get.ElevenMonthWarrantySurvey(new {StartDate = DateTime.Today.ToFirstDay(), EndDate = DateTime.Today.ToLastDay(), EmployeeId = ""});
            List<ApiResult> thisMonthSurveys = thisMonthRawSurveys.Details.ToObject<List<ApiResult>>();
            var thisMonthSurveysInMarket = thisMonthSurveys.Where(x => employees.Contains(x.WarrantyServiceRepresentativeEmployeeId)).ToList();

            var totalThisMonthSurveys = thisMonthSurveysInMarket.Count();
            var totalThisMonthSurveysWithRecommend = thisMonthSurveysInMarket.Count(x => string.Equals(x.DefinitelyWillRecommend, SurveyConstants.DefinitelyWillThreshold, StringComparison.CurrentCultureIgnoreCase));

            var lastMonthRawSurveys = _surveyClient.Get.ElevenMonthWarrantySurvey(new { StartDate = DateTime.Today.AddMonths(-1).ToFirstDay(), EndDate = DateTime.Today.AddMonths(-1).ToLastDay(), EmployeeId = "" });
            List<ApiResult> lastMonthSurveys = lastMonthRawSurveys.Details.ToObject<List<ApiResult>>();
            var lastMonthSurveysInMarket = lastMonthSurveys.Where(x => employees.Contains(x.WarrantyServiceRepresentativeEmployeeId)).ToList();

            var totalLastMonthSurveys = lastMonthSurveysInMarket.Count();
            var totalLastMonthSurveysWithRecommend = lastMonthSurveysInMarket.Count(x => string.Equals(x.DefinitelyWillRecommend, SurveyConstants.DefinitelyWillThreshold, StringComparison.CurrentCultureIgnoreCase));

            return new PercentSurveyRecommendWidgetModel
            {
                PercentRecommendLastMonth = totalLastMonthSurveysWithRecommend * 100 / totalLastMonthSurveys,
                PercentRecommendThisMonth = totalThisMonthSurveysWithRecommend*100/totalThisMonthSurveys,
            };
        }

        private List<string> GetEmployeesInMarket()
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
                return employeesInMarket;
            }
        } 

        internal class ApiResult
        {
            public string WarrantyServiceRepresentativeEmployeeId { get; set; }
            public string DefinitelyWillRecommend { get; set; }
        }
    }
}