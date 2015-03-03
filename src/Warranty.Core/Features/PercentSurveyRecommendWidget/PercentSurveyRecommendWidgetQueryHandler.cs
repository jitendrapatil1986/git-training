namespace Warranty.Core.Features.PercentSurveyRecommendWidget
{
    using System;
    using System.Collections.Generic;
    using Configurations;
    using Extensions;
    using Services;
    using System.Linq;

    public class PercentSurveyRecommendWidgetQueryHandler : IQueryHandler<PercentSurveyRecommendWidgetQuery, PercentSurveyRecommendWidgetModel>
    {
        private readonly IEmployeeService _employeeService;
        private readonly ISurveyService _surveyService;

        public PercentSurveyRecommendWidgetQueryHandler(IEmployeeService employeeService, ISurveyService surveyService)
        {
            _employeeService = employeeService;
            _surveyService = surveyService;
        }

        public PercentSurveyRecommendWidgetModel Handle(PercentSurveyRecommendWidgetQuery query)
        {
            var employees = _employeeService.GetEmployeesInMarket();

            var thisMonthRawSurveys = _surveyService.Execute(x => x.Get.ElevenMonthWarrantySurvey( new {
                                                                                                    StartDate = SystemTime.Today.ToFirstDay(),
                                                                                                    EndDate = SystemTime.Today.ToLastDay(),
                                                                                                    EmployeeIds = employees}));

            var thisMonthSurveysInMarket = new List<ApiResult>();
            if (thisMonthRawSurveys != null)
            {
                thisMonthSurveysInMarket = thisMonthRawSurveys.Details.ToObject<List<ApiResult>>();
            }
            var totalThisMonthSurveys = thisMonthSurveysInMarket.Count();
            var totalThisMonthSurveysWithRecommend = thisMonthSurveysInMarket.Count(x => string.Equals(x.DefinitelyWillRecommend, SurveyConstants.DefinitelyWillThreshold, StringComparison.CurrentCultureIgnoreCase));

            var lastMonthRawSurveys = _surveyService.Execute(x => x.Get.ElevenMonthWarrantySurvey(new {
                                                                                                    StartDate = SystemTime.Today.AddMonths(-1).ToFirstDay(),
                                                                                                    EndDate = SystemTime.Today.AddMonths(-1).ToLastDay(),
                                                                                                    EmployeeIds = employees}));

            var lastMonthSurveysInMarket = new List<ApiResult>();
            if (lastMonthRawSurveys != null)
            {
                lastMonthSurveysInMarket = lastMonthRawSurveys.Details.ToObject<List<ApiResult>>();
            }

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

        internal class ApiResult
        {
            public string WarrantyServiceRepresentativeEmployeeId { get; set; }
            public string DefinitelyWillRecommend { get; set; }
        }
    }
}