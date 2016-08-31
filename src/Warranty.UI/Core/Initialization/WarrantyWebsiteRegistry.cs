﻿using AutoMapper;
using Common.Security.Session;
using Common.UI.Security.Session;
using FluentValidation;
using FluentValidation.Mvc;
using log4net;
using Warranty.UI.Core.Automapper;

namespace Warranty.UI.Core.Initialization
{
    using System;
    using System.Configuration;
    using System.Web;
    using System.Web.Mvc;
    using Accounting.Client;
    using Mailers;
    using StructureMap.Configuration.DSL;
    using StructureMap.Pipeline;
    using Survey.Client;
    using Warranty.Core;
    using Warranty.Core.Extensions;
    using Warranty.Core.FileManagement;
    using Warranty.Core.Services;

    public class WarrantyWebsiteRegistry : Registry
    {
        public WarrantyWebsiteRegistry()
        {
            For<IValidatorFactory>().Use<StructureMapValidatorFactory>();
            For<ModelValidatorProvider>().Use<FluentValidationModelValidatorProvider>();
            For<HttpRequest>().Use(() => HttpContext.Current.Request);
            For<IUserSession>().HttpContextScoped().Use<WebUserSession>();
            Forward<IUserSession, IWebUserSession>();
            For<IWarrantyMailer>().Use<WarrantyMailer>();
            For<IManageToDoFilterCookie>().Use<ToDoFilterCookieManager>();
            For(typeof(IFileSystemManager<>)).Use(typeof(FileSystemManager<>));
            For<IMapper>().Singleton().Use(cfg => CreateMapper());

            var baseAccountingApiUri = ConfigurationManager.AppSettings["Accounting.API.BaseUri"];
            var accountingTimeoutInMilliseconds = ConfigurationManager.AppSettings["Accounting.API.TimeoutInMilliseconds"];
            var accountingTimeout = accountingTimeoutInMilliseconds.TryParseNullable();
            
            var accountingFailures = ConfigurationManager.AppSettings["Accounting.API.FailuresToAssumeDown"];
            var accountingFailuresToAssumeDown = accountingFailures.TryParseNullable() ?? 3;

            var accountingTimeToTryAgainInSeconds = ConfigurationManager.AppSettings["Accounting.API.TimeToTryAgainInSeconds"];
            var accountingTimeToTryAgain = accountingTimeToTryAgainInSeconds.TryParseNullable() ?? 30;

            For<AccountingClientSystemMonitor>()
                .LifecycleIs(new HybridLifecycle())
                .Use(() => new AccountingClientSystemMonitor(accountingFailuresToAssumeDown, new TimeSpan(0, 0, 0, accountingTimeToTryAgain)));

            For<AccountingClientConfiguration>()
                .Singleton()
                .Use(() => new AccountingClientConfiguration(baseAccountingApiUri, accountingTimeout));

            var baseSurveyApiUri = ConfigurationManager.AppSettings["Survey.API.BaseUri"];
            var surveyTimeoutInMilliseconds = ConfigurationManager.AppSettings["Survey.API.TimeoutInMilliseconds"];
            var surveyTimeout = surveyTimeoutInMilliseconds.TryParseNullable();

            var surveyFailures = ConfigurationManager.AppSettings["Survey.API.FailuresToAssumeDown"];
            var surveyFailuresToAssumeDown = surveyFailures.TryParseNullable() ?? 3;

            var surveyTimeToTryAgainInSeconds = ConfigurationManager.AppSettings["Survey.API.TimeToTryAgainInSeconds"];
            var surveyTimeToTryAgain = surveyTimeToTryAgainInSeconds.TryParseNullable() ?? 30;

            For<SurveyClientSystemMonitor>()
                .LifecycleIs(new HybridLifecycle())
                .Use(ctx => new SurveyClientSystemMonitor(surveyFailuresToAssumeDown, new TimeSpan(0, 0, 0, surveyTimeToTryAgain), LogManager.GetLogger("SurveyLog")));

            For<SurveyClientConfiguration>()
                .Singleton()
                .Use(() => new SurveyClientConfiguration(baseSurveyApiUri, surveyTimeout));

            For<ISurveyService>()
                .Use<SurveyService>()
                .Ctor<ILog>().Is(LogManager.GetLogger("SurveyLog"));
        }

        private static IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()); // add others if needed
            return config.CreateMapper();
        }
    }
}