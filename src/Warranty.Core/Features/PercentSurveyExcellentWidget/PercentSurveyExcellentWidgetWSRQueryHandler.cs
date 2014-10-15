namespace Warranty.Core.Features.PercentSurveyExcellentWidget
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configurations;
    using Extensions;
    using Security;
    using Survey.Client;

    public class PercentSurveyExcellentWidgetWSRQueryHandler : IQueryHandler<PercentSurveyExcellentWidgetWSRQuery, PercentSurveyExcellentWidgetModel>
    {
        private readonly IUserSession _userSession;
        private readonly ISurveyClient _surveyClient;

        public PercentSurveyExcellentWidgetWSRQueryHandler(IUserSession userSession, ISurveyClient surveyClient)
        {
            _userSession = userSession;
            _surveyClient = surveyClient;
        }

        public PercentSurveyExcellentWidgetModel Handle(PercentSurveyExcellentWidgetWSRQuery query)
        {
            var user = _userSession.GetCurrentUser();

            var thisMonthRawSurveys = _surveyClient.Get.ElevenMonthWarrantySurvey(new {StartDate = DateTime.Today.ToFirstDay(), EndDate = DateTime.Today.ToLastDay(), EmployeeId = user.EmployeeNumber});
            List<ApiResult> thisMonthSurveys = thisMonthRawSurveys.Details.ToObject<List<ApiResult>>();

            var totalThisMonthSurveys = thisMonthSurveys.Count();
            var totalThisMonthSurveysWithRecommend = thisMonthSurveys.Count(x => Convert.ToInt16(x.ExcellentWarrantyService) >= SurveyConstants.ExcellentWarrantyThreshold);

            var lastMonthRawSurveys = _surveyClient.Get.ElevenMonthWarrantySurvey(new { StartDate = DateTime.Today.AddMonths(-1).ToFirstDay(), EndDate = DateTime.Today.AddMonths(-1).ToLastDay(), EmployeeId = user.EmployeeNumber });
            List<ApiResult> lastMonthSurveys = lastMonthRawSurveys.Details.ToObject<List<ApiResult>>();

            var totalLastMonthSurveys = lastMonthSurveys.Count();
            var totalLastMonthSurveysWithRecommend = lastMonthSurveys.Count(x => Convert.ToInt16(x.ExcellentWarrantyService) >= SurveyConstants.ExcellentWarrantyThreshold);

            return new PercentSurveyExcellentWidgetModel
            {
                PercentExcellentLastMonth = totalLastMonthSurveysWithRecommend * 100 / totalLastMonthSurveys,
                PercentExcellentThisMonth = totalThisMonthSurveysWithRecommend * 100 / totalThisMonthSurveys,
            };
        }

        internal class ApiResult
        {
            public string WarrantyServiceRepresentativeEmployeeId { get; set; }
            public string ExcellentWarrantyService { get; set; }
        }
    }
}