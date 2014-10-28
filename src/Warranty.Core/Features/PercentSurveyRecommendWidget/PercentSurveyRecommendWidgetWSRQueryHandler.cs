namespace Warranty.Core.Features.PercentSurveyRecommendWidget
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configurations;
    using Extensions;
    using Security;
    using Survey.Client;

    public class PercentSurveyRecommendWidgetWSRQueryHandler : IQueryHandler<PercentSurveyRecommendWidgetWSRQuery, PercentSurveyRecommendWidgetModel>
    {
        private readonly IUserSession _userSession;
        private readonly ISurveyClient _surveyClient;

        public PercentSurveyRecommendWidgetWSRQueryHandler(IUserSession userSession, ISurveyClient surveyClient)
        {
            _userSession = userSession;
            _surveyClient = surveyClient;
        }

        public PercentSurveyRecommendWidgetModel Handle(PercentSurveyRecommendWidgetWSRQuery query)
        {
            var user = _userSession.GetCurrentUser();

            var thisMonthRawSurveys = _surveyClient.Get.ElevenMonthWarrantySurvey(new {StartDate = DateTime.Today.ToFirstDay(), EndDate = DateTime.Today.ToLastDay(), EmployeeId = user.EmployeeNumber});
            List<ApiResult> thisMonthSurveys = thisMonthRawSurveys.Details.ToObject<List<ApiResult>>();

            var totalThisMonthSurveys = thisMonthSurveys.Count();
            var totalThisMonthSurveysWithRecommend = thisMonthSurveys.Count(x => string.Equals(x.DefinitelyWillRecommend, SurveyConstants.DefinitelyWillThreshold, StringComparison.CurrentCultureIgnoreCase));

            var lastMonthRawSurveys = _surveyClient.Get.ElevenMonthWarrantySurvey(new { StartDate = DateTime.Today.AddMonths(-1).ToFirstDay(), EndDate = DateTime.Today.AddMonths(-1).ToLastDay(), EmployeeId = user.EmployeeNumber });
            List<ApiResult> lastMonthSurveys = lastMonthRawSurveys.Details.ToObject<List<ApiResult>>();

            var totalLastMonthSurveys = lastMonthSurveys.Count();
            var totalLastMonthSurveysWithRecommend = lastMonthSurveys.Count(x => string.Equals(x.DefinitelyWillRecommend, SurveyConstants.DefinitelyWillThreshold, StringComparison.CurrentCultureIgnoreCase));

            return new PercentSurveyRecommendWidgetModel
            {
                PercentRecommendLastMonth = GetPercentage(totalLastMonthSurveysWithRecommend, totalLastMonthSurveys),
                PercentRecommendThisMonth = GetPercentage(totalThisMonthSurveysWithRecommend, totalThisMonthSurveys),
            };
        }

        private int GetPercentage(int totalWithRecommend, int total)
        {
            if (totalWithRecommend == 0 || total == 0)
            {
                return 0;
            }
            return totalWithRecommend * 100 / total;
        }

        internal class ApiResult
        {
            public string WarrantyServiceRepresentativeEmployeeId { get; set; }
            public string DefinitelyWillRecommend { get; set; }
        }
    }
}