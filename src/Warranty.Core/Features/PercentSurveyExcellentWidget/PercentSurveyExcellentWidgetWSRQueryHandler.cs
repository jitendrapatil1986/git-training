namespace Warranty.Core.Features.PercentSurveyExcellentWidget
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configurations;
    using Extensions;
    using Security;
    using Services;

    public class PercentSurveyExcellentWidgetWSRQueryHandler : IQueryHandler<PercentSurveyExcellentWidgetWSRQuery, PercentSurveyExcellentWidgetModel>
    {
        private readonly IUserSession _userSession;
        private readonly ISurveyService _surveyService;

        public PercentSurveyExcellentWidgetWSRQueryHandler(IUserSession userSession, ISurveyService surveyService)
        {
            _userSession = userSession;
            _surveyService = surveyService;
        }

        public PercentSurveyExcellentWidgetModel Handle(PercentSurveyExcellentWidgetWSRQuery query)
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

            var totalThisMonthSurveys = thisMonthSurveys.Count();
            var totalThisMonthSurveysWithRecommend = thisMonthSurveys.Count(x => Convert.ToInt16(x.ExcellentWarrantyService) >= SurveyConstants.ExcellentWarrantyThreshold);

            var lastMonthRawSurveys = _surveyService.Execute(x => x.Get.ElevenMonthWarrantySurvey(new {
                                                                                                    StartDate = DateTime.Today.AddMonths(-1).ToFirstDay(),
                                                                                                    EndDate = DateTime.Today.AddMonths(-1).ToLastDay(),
                                                                                                    EmployeeId = user.EmployeeNumber}));

            var lastMonthSurveys = new List<ApiResult>();
            if (lastMonthRawSurveys != null)
            {
                lastMonthSurveys = lastMonthRawSurveys.Details.ToObject<List<ApiResult>>();
            }

            var totalLastMonthSurveys = lastMonthSurveys.Count();
            var totalLastMonthSurveysWithRecommend = lastMonthSurveys.Count(x => Convert.ToInt16(x.ExcellentWarrantyService) >= SurveyConstants.ExcellentWarrantyThreshold);

            return new PercentSurveyExcellentWidgetModel
            {
                PercentExcellentLastMonth = ServiceCallCalculator.CalculatePercentage( totalLastMonthSurveysWithRecommend, totalLastMonthSurveys),
                TotalSurveysLastMonth = totalLastMonthSurveys,
                PercentExcellentThisMonth = ServiceCallCalculator.CalculatePercentage(totalThisMonthSurveysWithRecommend, totalThisMonthSurveys),
                TotalSurveysThisMonth = totalThisMonthSurveys,
            };
        }

        internal class ApiResult
        {
            public string WarrantyServiceRepresentativeEmployeeId { get; set; }
            public string ExcellentWarrantyService { get; set; }
        }
    }
}