namespace Warranty.Server.Handlers.Jobs
{
    using System.Linq;
    using Common.DataAccess;
    using Core.Entities;
    using Land.Events;
    using NServiceBus;

    public class JobAddressChangedHandler : IHandleMessages<LotAddressChanged>
    {
        private readonly IRepository _repository;

        public JobAddressChangedHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(LotAddressChanged message)
        {
            var job = _repository.Query<Job>().Single(j => j.JobNumber == message.JdeIdentifier);

            //We may not have the job in warranty yet so just return in that case
            if (job == null)
                return;

            job.AddressLine = string.Format("{0} {1}", message.NewStreetNumber, message.NewStreetName);

            _repository.SaveOrUpdate(job);
        }
    }
}