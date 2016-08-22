using log4net;

namespace Warranty.Core.Services
{
    using System;
    using System.Linq.Expressions;
    using Survey.Client;

    public class SurveyService : ISurveyService
    {
        private readonly ISurveyClient _surveyClient;
        private readonly SurveyClientSystemMonitor _monitor;
        private readonly ILog _log;

        public SurveyService(ISurveyClient surveyClient, SurveyClientSystemMonitor monitor, ILog log)
        {
            _surveyClient = surveyClient;
            _monitor = monitor;
            _log = log;
        }

        public TResponse Execute<TResponse>(Expression<Func<ISurveyClient, TResponse>> expression)
        {
            try
            {
                var func = expression.Compile();
                _log.Info(expression.ToString());
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