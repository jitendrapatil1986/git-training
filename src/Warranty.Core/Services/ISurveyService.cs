namespace Warranty.Core.Services
{
    using System;
    using System.Linq.Expressions;
    using Survey.Client;

    public interface ISurveyService
    {
        TResponse Execute<TResponse>(Expression<Func<ISurveyClient, TResponse>> expression);
    }
}