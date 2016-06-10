using System;
using System.Collections.Generic;
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
    public class SoldJobsHealthCheckSaga : Saga<SoldJobsHealthCheckSagaData>,
        IAmStartedByMessages<InitiateSoldJobsHealthCheckSaga>,
        IHandleMessages<SoldJobsHealthCheckSaga_CleanupTempTables>,
        IHandleMessages<SoldJobsHealthCheckSaga_LoadSoldJobsFromTips>,
        IHandleMessages<SoldJobsHealthCheckSaga_LoadSoldJobsFromWarranty>,
        IHandleMessages<SoldJobsHealthCheckSaga_CompareJobsFromTipsAndWarranty>
    {
        private readonly ILog _log;
        private readonly IMediator _mediator;
        public SoldJobsHealthCheckSaga() { }

        public SoldJobsHealthCheckSaga(ILog log, IMediator mediator)
        {
            _log = log;
            _mediator = mediator;
        }

        public override void ConfigureHowToFindSaga()
        {
            ConfigureMapping<InitiateSoldJobsHealthCheckSaga>(m => m.RunDate).ToSaga(s => s.RunDate);
            ConfigureMapping<SoldJobsHealthCheckSaga_CleanupTempTables>(m => m.RunDate).ToSaga(s => s.RunDate);
            ConfigureMapping<SoldJobsHealthCheckSaga_LoadSoldJobsFromTips>(m => m.RunDate).ToSaga(s => s.RunDate);
            ConfigureMapping<SoldJobsHealthCheckSaga_LoadSoldJobsFromWarranty>(m => m.RunDate).ToSaga(s => s.RunDate);
            ConfigureMapping<SoldJobsHealthCheckSaga_CompareJobsFromTipsAndWarranty>(m => m.RunDate).ToSaga(s => s.RunDate);
        }

        public void Handle(InitiateSoldJobsHealthCheckSaga message)
        {
            // Safety check to make sure we don't step on ourselves
            if (Data.RunDate.HasValue)
            {
                _log.WarnFormat("Trying to start a new instance of the SoldJobsHealthCheckSaga but it looks like one is already running for {0:d}.  If it's possible stuck, you can delete the SagaData record and try again.", message.RunDate);
                return;
            }

            _log.InfoFormat("Started new instance of the SoldJobsHealthCheckSaga for {0:d}", Data.RunDate);
            Data.RunDate = message.RunDate.Date;
            Data.OldestSaleDate = Data.RunDate.Value.AddDays(-365);
            Bus.SendLocal(new SoldJobsHealthCheckSaga_CleanupTempTables(Data.RunDate.Value));
        }

        public void Handle(SoldJobsHealthCheckSaga_CleanupTempTables message)
        {
            _log.Info("Clearing any existing data from previous checks");
            _mediator.Send(new ClearTempJobNumberTablesRequest());

            Bus.SendLocal(new SoldJobsHealthCheckSaga_LoadSoldJobsFromTips(Data.RunDate.Value));
        }

        public void Handle(SoldJobsHealthCheckSaga_LoadSoldJobsFromTips message)
        {
            _log.InfoFormat("Loading all sold jobs from Tips newer than {0:d}", Data.RunDate);
            _mediator.Send(new LoadSoldJobsFromTipsRequest(Data.OldestSaleDate));

            Bus.SendLocal(new SoldJobsHealthCheckSaga_LoadSoldJobsFromWarranty(Data.RunDate.Value));
        }

        public void Handle(SoldJobsHealthCheckSaga_LoadSoldJobsFromWarranty message)
        {
            _log.Info("Loading all jobs from Warranty");
            _mediator.Send(new LoadJobsFromWarrantyRequest());

            Bus.SendLocal(new SoldJobsHealthCheckSaga_CompareJobsFromTipsAndWarranty(Data.RunDate.Value));
        }

        public void Handle(SoldJobsHealthCheckSaga_CompareJobsFromTipsAndWarranty message)
        {
            var tipsJobs = _mediator.Send(new GetSoldJobsRequest(Systems.TIPS));
            var warrantyJobs = _mediator.Send(new GetSoldJobsRequest(Systems.Warranty));

            var missingJobs = tipsJobs.Except(warrantyJobs).ToList();

            if (!missingJobs.Any())
            {
                _log.Info("Could not find any jobs missing in Warranty");
                MarkAsComplete();
                return;
            }

            _log.WarnFormat("Found {0} missing jobs from Warranty", missingJobs.Count());
            
            var notification = new StringBuilder();
            notification.AppendLine("Found the following sold jobs missing from Warranty:<br>");
            notification.AppendLine("<hr>");

            foreach (var missingJob in missingJobs)
                notification.AppendFormat("{0}<br>\n", missingJob);

            Bus.SendLocal(new Notification
            {
                Subject = string.Format("{0} - HEALTH CHECK FAILURE - {1} missing sold jobs in Warranty", Settings.DatabaseName.Value, missingJobs.Count),
                Body = notification.ToString()
            });
            MarkAsComplete();
        }

        
    }

    public class SoldJobsHealthCheckSaga_CleanupTempTables : IBusCommand
    {
        public DateTime RunDate { get; set; }

        public SoldJobsHealthCheckSaga_CleanupTempTables() { }

        public SoldJobsHealthCheckSaga_CleanupTempTables(DateTime runDate)
        {
            RunDate = runDate;
        }
    }

    public class SoldJobsHealthCheckSaga_CompareJobsFromTipsAndWarranty : IBusCommand
    {
        public DateTime RunDate { get; set; }

        public SoldJobsHealthCheckSaga_CompareJobsFromTipsAndWarranty() { }

        public SoldJobsHealthCheckSaga_CompareJobsFromTipsAndWarranty(DateTime runDate)
        {
            RunDate = runDate;
        }
    }


    public class SoldJobsHealthCheckSaga_LoadSoldJobsFromWarranty : IBusCommand
    {
        public DateTime RunDate { get; set; }

        public SoldJobsHealthCheckSaga_LoadSoldJobsFromWarranty() { }

        public SoldJobsHealthCheckSaga_LoadSoldJobsFromWarranty(DateTime runDate)
        {
            RunDate = runDate;
        }
    }

    public class SoldJobsHealthCheckSaga_LoadSoldJobsFromTips : IBusCommand
    {
        public DateTime RunDate { get; set; }

        public SoldJobsHealthCheckSaga_LoadSoldJobsFromTips() { }

        public SoldJobsHealthCheckSaga_LoadSoldJobsFromTips(DateTime runDate)
        {
            RunDate = runDate;
        }
    }

    public class InitiateSoldJobsHealthCheckSaga : IBusCommand
    {
        public DateTime RunDate { get; set; }

        public InitiateSoldJobsHealthCheckSaga() { }

        public InitiateSoldJobsHealthCheckSaga(DateTime runDate)
        {
            RunDate = runDate;
        }
    }

    public class SoldJobsHealthCheckSagaData : IContainSagaData
    {
        public virtual Guid Id { get; set; }
        public virtual string Originator { get; set; }
        public virtual string OriginalMessageId { get; set; }
        [Unique]
        public virtual DateTime? RunDate { get; set; }
        public virtual DateTime OldestSaleDate { get; set; }
    }
}