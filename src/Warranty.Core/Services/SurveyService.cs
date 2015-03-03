namespace Warranty.Core.Services
{
    using System;
    using System.Linq.Expressions;
    using Survey.Client;

    public class SurveyService : ISurveyService
    {
        private readonly ISurveyClient _surveyClient;
        private readonly SurveyClientSystemMonitor _monitor;

        public SurveyService(ISurveyClient surveyClient, SurveyClientSystemMonitor monitor)
        {
            _surveyClient = surveyClient;
            _monitor = monitor;
        }

        public TResponse Execute<TResponse>(Expression<Func<ISurveyClient, TResponse>> expression)
        {
            try
            {
                var func = expression.Compile();
                return func(_surveyClient);
            }
            catch (Exception ex)
            {
                _monitor.LogException(expression.Body.ToString(), ex);
                return default(TResponse);
            }
        }
    }
}