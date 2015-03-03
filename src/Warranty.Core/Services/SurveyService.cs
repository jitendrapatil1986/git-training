namespace Warranty.Core.Services
{
    using System;
    using System.Linq.Expressions;
    using Survey.Client;

    public class SurveyService : ISurveyService
    {
        private readonly ISurveyClient _surveyClient;

        public SurveyService(ISurveyClient surveyClient)
        {
            _surveyClient = surveyClient;
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
                return default(TResponse);
            }
        }
    }
}