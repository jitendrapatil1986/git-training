namespace Warranty.Core.Features.PercentSurveyExcellentWidget
{
    using System;
    using System.Collections.Generic;
    using Configurations;
    using Extensions;
    using Services;
    using System.Linq;

    public class PercentSurveyExcellentWidgetQueryHandler : IQueryHandler<PercentSurveyExcellentWidgetQuery, PercentSurveyExcellentWidgetModel>
    {
        private readonly IEmployeeService _employeeService;
        private readonly ISurveyService _surveyService;

        public PercentSurveyExcellentWidgetQueryHandler(IEmployeeService employeeService, ISurveyService surveyService)
        {
            _employeeService = employeeService;
            _surveyService = surveyService;
        }

        public PercentSurveyExcellentWidgetModel Handle(PercentSurveyExcellentWidgetQuery query)
        {
            var employees = _employeeService.GetEmployeesInMarket();
            var thisMonthRawSurveys = _surveyService.Execute(x => x.Get.ElevenMonthWarrantySurvey(new {
                                                                                                        StartDate = SystemTime.Today.ToFirstDay(),
                                                                                                        EndDate = SystemTime.Today.ToLastDay(),
                                                                                                        EmployeeIds = employees}));

            var thisMonthSurveysInMarket = new List<ApiResult>();
            if (thisMonthRawSurveys != null)
            {
                thisMonthSurveysInMarket = thisMonthRawSurveys.Details.ToObject<List<ApiResult>>();
            }
            
            var totalThisMonthSurveys = thisMonthSurveysInMarket.Count();
            var totalThisMonthSurveysWithRecommend = thisMonthSurveysInMarket.Count(x => Convert.ToInt16(x.ExcellentWarrantyService) >= SurveyConstants.ExcellentWarrantyThreshold);

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
            var totalLastMonthSurveysWithRecommend = lastMonthSurveysInMarket.Count(x => Convert.ToInt16(x.ExcellentWarrantyService) >= SurveyConstants.ExcellentWarrantyThreshold);

            return new PercentSurveyExcellentWidgetModel
            {
                PercentExcellentLastMonth = ServiceCallCalculator.CalculatePercentage(totalLastMonthSurveysWithRecommend, totalLastMonthSurveys),
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