using Common.Api.Http;
using FluentValidation;
using Warranty.Core.AccountingApiHelpers;
using Warranty.Core.ActivityLogger;
using Warranty.Core.ApprovalInfrastructure.Interfaces;

namespace Warranty.Core
{
    using System.Configuration;
    using DataAccess;
    using Entities;
    using Extensions;
    using NPoco;
    using NPoco.FluentMappings;
    using StructureMap.Configuration.DSL;
    using Survey.Client;

    public class WarrantyCoreRegistry : Registry
    {
        public WarrantyCoreRegistry()
        {
            Scan(x =>
                     {
                         x.AssemblyContainingType<IAuditableEntity>();
                         x.AssemblyContainingType<ISurveyClient>();
                         x.AddAllTypesOf<IMap>();
                         x.AssemblyContainingType<IMediator>();
                         x.AddAllTypesOf(typeof(IValidator<>));
                         x.AddAllTypesOf((typeof(IQueryHandler<,>)));
                         x.AddAllTypesOf((typeof(ICommandHandler<>)));
                         x.AddAllTypesOf((typeof(ICommandHandler<,>)));
                         x.AddAllTypesOf((typeof(ICommandResultHandler<,>)));
                         x.AddAllTypesOf((typeof(IApprovalService<>)));
                         x.AddAllTypesOf((typeof(IActivityLogger)));
                         x.WithDefaultConventions();
                     });

            For<IDatabase>().Use(() => DbFactory.DatabaseFactory.GetDatabase());

            var baseSurveyApiUri = ConfigurationManager.AppSettings["Survey.API.BaseUri"];
            var timeoutInMilliseconds = ConfigurationManager.AppSettings["Survey.API.TimeoutInMilliseconds"];
            var timeout = timeoutInMilliseconds.TryParseNullable();

            For<SurveyClientConfiguration>().Singleton().Use(() => new SurveyClientConfiguration(baseSurveyApiUri, timeout));

            For<HttpApiClient>().Use<HttpApiClient>();
            For<ApiJsonConverter>().Use<ApiJsonConverter>();
        }
    }
}
