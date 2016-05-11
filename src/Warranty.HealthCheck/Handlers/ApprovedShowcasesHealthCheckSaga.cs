using System;
using System.Configuration;
using System.Linq;
using System.Text;
using Common.Messages;
using log4net;
using MediatR;
using NServiceBus;
using NServiceBus.Saga;
using Warranty.HealthCheck.Mediatr;
using Warranty.HealthCheck.Models;

namespace Warranty.HealthCheck.Handlers
{
    public class ApprovedShowcasesHealthCheckSaga : Saga<ApprovedShowcasesHealthCheckSagaData>,
        IAmStartedByMessages<InitiateApprovedShowcasesHealthCheckSaga>,
        IHandleMessages<ApprovedShowcasesHealthCheckSaga_ClearShowcasesFromTempTable>,
        IHandleMessages<ApprovedShowcasesHealthCheckSaga_GetApprovedShowcasesFromTips>,
        IHandleMessages<ApprovedShowcasesHealthCheckSaga_GetShowcasesFromWarranty>,
        IHandleMessages<ApprovedShowcasesHealthCheckSaga_CompareShowcasesFromTipsAndWarranty>
    {
        private readonly ILog _log;
        private readonly IMediator _mediator;

        public ApprovedShowcasesHealthCheckSaga() { } // required for NSB

        public ApprovedShowcasesHealthCheckSaga(ILog log, IMediator mediator)
        {
            _log = log;
            _mediator = mediator;
        }

        public override void ConfigureHowToFindSaga()
        {
            ConfigureMapping<ApprovedShowcasesHealthCheckSaga_ClearShowcasesFromTempTable>(message => message.Running).ToSaga(data => data.Running);
            ConfigureMapping<ApprovedShowcasesHealthCheckSaga_GetApprovedShowcasesFromTips>(message => message.Running).ToSaga(data => data.Running);
            ConfigureMapping<ApprovedShowcasesHealthCheckSaga_GetShowcasesFromWarranty>(message => message.Running).ToSaga(data => data.Running);
            ConfigureMapping<ApprovedShowcasesHealthCheckSaga_CompareShowcasesFromTipsAndWarranty>(message => message.Running).ToSaga(data => data.Running);
        }

        public void Handle(InitiateApprovedShowcasesHealthCheckSaga message)
        {
            if (Data.Running)
            {
                _log.Info("Received message to start the ApprovedShowcasesHealthCheckSaga but there is already and instance running");
                return;
            }

            Data.Running = true;
            Bus.SendLocal(new ApprovedShowcasesHealthCheckSaga_ClearShowcasesFromTempTable(true));
        }
        public void Handle(ApprovedShowcasesHealthCheckSaga_ClearShowcasesFromTempTable message)
        {
            _log.Info("Clearing any existing data from previous checks");
            _mediator.Send(new ClearTempShowcasesTable());

            Bus.SendLocal(new ApprovedShowcasesHealthCheckSaga_GetApprovedShowcasesFromTips());
        }

        public void Handle(ApprovedShowcasesHealthCheckSaga_GetApprovedShowcasesFromTips message)
        {
            _log.Info("Fetching approved showcases from TIPS");
            _mediator.Send(new LoadApprovedShowcasesFromTips());

            Bus.SendLocal(new ApprovedShowcasesHealthCheckSaga_GetShowcasesFromWarranty());
        }

        public void Handle(ApprovedShowcasesHealthCheckSaga_GetShowcasesFromWarranty message)
        {
            _log.Info("Fetching showcases from Warranty");
            _mediator.Send(new LoadShowcasesFromWarranty());

            Bus.SendLocal(new ApprovedShowcasesHealthCheckSaga_CompareShowcasesFromTipsAndWarranty());
        }

        public void Handle(ApprovedShowcasesHealthCheckSaga_CompareShowcasesFromTipsAndWarranty message)
        {
            _log.Info("Comparing data from TIPS for anything missing in Warranty");

            var tipsShowcases = _mediator.Send(new GetShowcases(Systems.TIPS));
            var warrantyShowcases = _mediator.Send(new GetShowcases(Systems.Warranty));

            var missingShowcases = tipsShowcases.Except(warrantyShowcases).ToList();

            if (!missingShowcases.Any())
            {
                _log.Info("Could not find any showcases missing in Warranty");
                MarkAsComplete();
                return;
            }

            _log.WarnFormat("Found {0} missing showcases from Warranty", missingShowcases.Count());

            var notification = new StringBuilder();
            notification.AppendLine("Found the following showcases missing from Warranty:<br>");
            notification.AppendLine("<hr>");

            foreach (var missingShowcase in missingShowcases)
                notification.AppendFormat("{0}<br>\n", missingShowcase);

            Bus.SendLocal(new Notification
            {
                Subject = string.Format("{0} - HEALTH CHECK FAILURE - {1} missing showcases in Warranty", Settings.DatabaseName.Value, missingShowcases.Count),
                Body = notification.ToString()
            });
            MarkAsComplete();
        }
    }

    public class ApprovedShowcasesHealthCheckSaga_ClearShowcasesFromTempTable : IBusCommand
    {
        public ApprovedShowcasesHealthCheckSaga_ClearShowcasesFromTempTable(bool running)
        {
            Running = running;
        }

        public ApprovedShowcasesHealthCheckSaga_ClearShowcasesFromTempTable() { }

        public bool Running { get; set; }
    }

    public class ApprovedShowcasesHealthCheckSaga_CompareShowcasesFromTipsAndWarranty : IBusCommand
    {
        public ApprovedShowcasesHealthCheckSaga_CompareShowcasesFromTipsAndWarranty(bool running)
        {
            Running = running;
        }

        public ApprovedShowcasesHealthCheckSaga_CompareShowcasesFromTipsAndWarranty() { }

        public bool Running { get; set; }
    }

    public class ApprovedShowcasesHealthCheckSaga_GetShowcasesFromWarranty : IBusCommand
    {
        public ApprovedShowcasesHealthCheckSaga_GetShowcasesFromWarranty(bool running)
        {
            Running = running;
        }

        public ApprovedShowcasesHealthCheckSaga_GetShowcasesFromWarranty() { }

        public bool Running { get; set; }
    }

    public class ApprovedShowcasesHealthCheckSaga_GetApprovedShowcasesFromTips : IBusCommand
    {
        public ApprovedShowcasesHealthCheckSaga_GetApprovedShowcasesFromTips(bool running)
        {
            Running = running;
        }

        public ApprovedShowcasesHealthCheckSaga_GetApprovedShowcasesFromTips() { }

        public bool Running { get; set; }
    }

    public class InitiateApprovedShowcasesHealthCheckSaga : IBusCommand
    {
        public InitiateApprovedShowcasesHealthCheckSaga(bool running)
        {
            Running = running;
        }

        public InitiateApprovedShowcasesHealthCheckSaga() { }

        public bool Running { get; set; }
    }

    public class ApprovedShowcasesHealthCheckSagaData : IContainSagaData
    {
        public virtual Guid Id { get; set; }
        public virtual string Originator { get; set; }
        public virtual string OriginalMessageId { get; set; }
        public virtual bool Running { get; set; }
    }
}