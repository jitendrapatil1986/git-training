namespace Warranty.Core.Features.PercentSurveyOutstandingWidget
{
    using System;
    using System.Collections.Generic;
    using Configurations;
    using Extensions;
    using Services;
    using System.Linq;

    public class PercentSurveyOutstandingWidgetQueryHandler : IQueryHandler<PercentSurveyOutstandingWidgetQuery, PercentSurveyOutstandingWidgetModel>
    {
        private readonly IEmployeeService _employeeService;
        private readonly ISurveyService _surveyService;

        public PercentSurveyOutstandingWidgetQueryHandler(IEmployeeService employeeService, ISurveyService surveyService)
        {
            _employeeService = employeeService;
            _surveyService = surveyService;
        }

        public PercentSurveyOutstandingWidgetModel Handle(PercentSurveyOutstandingWidgetQuery query)
        {
            var employees = _employeeService.GetEmployeesInMarket();
            var thisMonthRawSurveys = _surveyService.Execute(x => x.Post.ElevenMonthWarrantySurvey(new {
                                                                                                        StartDate = SystemTime.Today.ToFirstDay(),
                                                                                                        EndDate = SystemTime.Today.ToLastDay(),
                                                                                                        EmployeeIds = employees}));

            var thisMonthSurveysInMarket = new List<ApiResult>();
            if (thisMonthRawSurveys != null)
            {
                thisMonthSurveysInMarket = thisMonthRawSurveys.Details.ToObject<List<ApiResult>>();
                thisMonthSurveysInMarket = thisMonthSurveysInMarket.Where(s => !string.IsNullOrWhiteSpace(s.WarrantyServiceScore) && !s.WarrantyServiceScore.Equals("N/A", StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var totalThisMonthSurveys =
               thisMonthSurveysInMarket.Count(x => !string.IsNullOrEmpty(x.WarrantyServiceScore));

            var totalThisMonthOutstandingServiceSurveys = thisMonthSurveysInMarket
                .Count(x => Convert.ToInt16(x.WarrantyServiceScore) >= SurveyConstants.OutstandingWarrantyThreshold);

            var lastMonthRawSurveys = _surveyService.Execute(x => x.Post.ElevenMonthWarrantySurvey(new {
                                                                                                        StartDate = SystemTime.Today.AddMonths(-1).ToFirstDay(),
                                                                                                        EndDate = SystemTime.Today.AddMonths(-1).ToLastDay(),
                                                                                                        EmployeeIds = employees}));

            var lastMonthSurveysInMarket = new List<ApiResult>();
            if (lastMonthRawSurveys != null)
            {
                lastMonthSurveysInMarket = lastMonthRawSurveys.Details.ToObject<List<ApiResult>>();
                lastMonthSurveysInMarket = lastMonthSurveysInMarket.Where(s => !string.IsNullOrWhiteSpace(s.WarrantyServiceScore) && !s.WarrantyServiceScore.Equals("N/A", StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var totalLastMonthSurveys =
                lastMonthSurveysInMarket.Count(x => !string.IsNullOrEmpty(x.WarrantyServiceScore));

            var totalLastMonthOutstandingServiceSurveys = lastMonthSurveysInMarket
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