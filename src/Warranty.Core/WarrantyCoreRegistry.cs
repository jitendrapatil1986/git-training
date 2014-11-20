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
                         x.WithDefaultConventions();
                     });

            For<IDatabase>().Use(() => DbFactory.DatabaseFactory.GetDatabase());

            var baseSurveyApiUri = ConfigurationManager.AppSettings["Survey.API.BaseUri"];
            var timeoutInMilliseconds = ConfigurationManager.AppSettings["Survey.API.TimeoutInMilliseconds"];
            var timeout = timeoutInMilliseconds.TryParseNullable();

            For<SurveyClientConfiguration>().Singleton().Use(() => new SurveyClientConfiguration(baseSurveyApiUri, timeout));
        }
    }
}
