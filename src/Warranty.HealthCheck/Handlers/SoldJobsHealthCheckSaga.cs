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

namespace Warranty.HealthCheck.Handlers
{
    public class SoldJobsHealthCheckSaga : Saga<SoldJobsHealthCheckSagaData>,
        IAmStartedByMessages<InitiateSoldJobsHealthCheckSaga>,
        IHandleMessages<SoldJobsHealthCheckSaga_GetSoldJobsFromTips>,
        IHandleMessages<SoldJobsHealthCheckSaga_GetSoldJobsFromWarranty>,
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
            ConfigureMapping<SoldJobsHealthCheckSaga_GetSoldJobsFromTips>(m => m.RunDate).ToSaga(s => s.RunDate);
            ConfigureMapping<SoldJobsHealthCheckSaga_GetSoldJobsFromWarranty>(m => m.RunDate).ToSaga(s => s.RunDate);
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
            Bus.SendLocal(new SoldJobsHealthCheckSaga_GetSoldJobsFromTips(Data.RunDate.Value));
        }

        public void Handle(SoldJobsHealthCheckSaga_GetSoldJobsFromTips message)
        {
            _log.InfoFormat("Getting all sold jobs from Tips {0:d}", Data.RunDate);
            Data.TipsJobs = _mediator.Send(new GetSoldJobsFromTipsRequest(Data.OldestSaleDate)).ToList();

            if (!Data.TipsJobs.Any())
            {
                _log.ErrorFormat("Could not find any sold jobs in TIPS from the last year - somethings not right!");
                MarkAsComplete();
                return;
            }

            _log.InfoFormat("Found {0} sold jobs in TIPS from the last year", Data.TipsJobs.Count);
            Bus.SendLocal(new SoldJobsHealthCheckSaga_GetSoldJobsFromWarranty(Data.RunDate.Value));
        }

        public void Handle(SoldJobsHealthCheckSaga_GetSoldJobsFromWarranty message)
        {
            _log.InfoFormat("Getting all jobs from Warranty");
            Data.WarrantyJobs = _mediator.Send(new GetJobsFromWarrantyRequest()).ToList();

            if (!Data.WarrantyJobs.Any())
            {
                _log.ErrorFormat("Could not find any jobs in Warranty - somethings not right!");
                MarkAsComplete();
                return;
            }

            _log.InfoFormat("Found {0} jobs in Warranty");
            Bus.SendLocal(new SoldJobsHealthCheckSaga_CompareJobsFromTipsAndWarranty(Data.RunDate.Value));
        }

        public void Handle(SoldJobsHealthCheckSaga_CompareJobsFromTipsAndWarranty message)
        {
            var missingJobs = Data.TipsJobs.Except(Data.WarrantyJobs).ToList();

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
                Subject = string.Format("{0} missing sold jobs in Warranty"),
                Body = notification.ToString()
            });
            MarkAsComplete();
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


    public class SoldJobsHealthCheckSaga_GetSoldJobsFromWarranty : IBusCommand
    {
        public DateTime RunDate { get; set; }

        public SoldJobsHealthCheckSaga_GetSoldJobsFromWarranty() { }

        public SoldJobsHealthCheckSaga_GetSoldJobsFromWarranty(DateTime runDate)
        {
            RunDate = runDate;
        }
    }

    public class SoldJobsHealthCheckSaga_GetSoldJobsFromTips : IBusCommand
    {
        public DateTime RunDate { get; set; }

        public SoldJobsHealthCheckSaga_GetSoldJobsFromTips() { }

        public SoldJobsHealthCheckSaga_GetSoldJobsFromTips(DateTime runDate)
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

        public virtual List<string> WarrantyJobs { get; set; }
        public virtual List<string> TipsJobs { get; set; }
    }
}