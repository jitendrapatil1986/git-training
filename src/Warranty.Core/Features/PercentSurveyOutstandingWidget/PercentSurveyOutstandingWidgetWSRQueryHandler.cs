namespace Warranty.Core.Features.PercentSurveyOutstandingWidget
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configurations;
    using Extensions;
    using Common.Security.Session;
    using Services;

    public class PercentSurveyOutstandingWidgetWSRQueryHandler : IQueryHandler<PercentSurveyOutstandingWidgetWSRQuery, PercentSurveyOutstandingWidgetModel>
    {
        private readonly IUserSession _userSession;
        private readonly ISurveyService _surveyService;

        public PercentSurveyOutstandingWidgetWSRQueryHandler(IUserSession userSession, ISurveyService surveyService)
        {
            _userSession = userSession;
            _surveyService = surveyService;
        }

        public PercentSurveyOutstandingWidgetModel Handle(PercentSurveyOutstandingWidgetWSRQuery query)
        {
            var user = _userSession.GetCurrentUser();

            var thisMonthRawSurveys = _surveyService.Execute(x => x.Get.ElevenMonthWarrantySurvey(new {
                                                                                                    StartDate = DateTime.Today.ToFirstDay(),
                                                                                                    EndDate = DateTime.Today.ToLastDay(),
                                                                                                    EmployeeId = user.EmployeeNumber}));

            var thisMonthSurveys = new List<ApiResult>();
            if (thisMonthRawSurveys != null)
            {
                thisMonthSurveys = thisMonthRawSurveys.Details.ToObject<List<ApiResult>>();
            }

            var totalThisMonthSurveys =
                thisMonthSurveys.Count(x => !string.IsNullOrEmpty(x.WarrantyServiceScore));

            var totalThisMonthOutstandingServiceSurveys = thisMonthSurveys
                .Count(x => Convert.ToInt16(x.WarrantyServiceScore) >= SurveyConstants.OutstandingWarrantyThreshold);

            var lastMonthRawSurveys = _surveyService.Execute(x => x.Get.ElevenMonthWarrantySurvey(new {
                                                                                                    StartDate = DateTime.Today.AddMonths(-1).ToFirstDay(),
                                                                                                    EndDate = DateTime.Today.AddMonths(-1).ToLastDay(),
                                                                                                    EmployeeId = user.EmployeeNumber}));

            var lastMonthSurveys = new List<ApiResult>();
            if (lastMonthRawSurveys != null)
            {
                lastMonthSurveys = lastMonthRawSurveys.Details.ToObject<List<ApiResult>>();
            }

            var totalLastMonthSurveys =
                lastMonthSurveys.Count(x => !string.IsNullOrEmpty(x.WarrantyServiceScore));

            var totalLastMonthOutstandingServiceSurveys = lastMonthSurveys
                .Count(x => Convert.ToInt16(x.WarrantyServiceScore) >= SurveyConstants.OutstandingWarrantyThreshold);

            return new PercentSurveyOutstandingWidgetModel
            {
                PercentOutstandingLastMonth = ServiceCallCalculator.CalculatePercentage(totalLastMonthOutstandingServiceSurveys, totalLastMonthSurveys),
                TotalSurveysLastMonth = totalLastMonthSurveys,
                PercentOutstandingThisMonth = ServiceCallCalculator.CalculatePercentage(totalThisMonthOutstandingServiceSurveys, totalThisMonthSurveys),
                TotalSurveysThisMonth = totalThisMonthSurveys,
            };
        }

        internal class ApiResult
        {
            public string WarrantyServiceRepresentativeEmployeeId { get; set; }
            public string WarrantyServiceScore { get; set; }
        }
    }
}