namespace Warranty.Core.Features.JobSummary.ChangeHomeowner
{
    using System;
    using Entities;
    using NPoco;
    using Security;

    public class ChangeHomeownerCommandHandler : ICommandHandler<ChangeHomeownerCommand>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public ChangeHomeownerCommandHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public void Handle(ChangeHomeownerCommand message)
        {
            using (_database)
            {
                const string sql = @"SELECT DISTINCT ho.HomeownerNumber
                                    FROM Jobs j
                                    INNER JOIN HomeOwners ho
                                    ON j.JobId = ho.JobId
                                    WHERE j.JobId = @0
                                    ORDER BY ho.HomeownerNumber DESC";

                var currentHomeownerNumber = _database.SingleOrDefault<int>(sql, message.Model.JobId);

                var newHomeowner = new HomeOwner
                    {
                        JobId = message.Model.JobId,
                        HomeOwnerNumber = currentHomeownerNumber + 1,
                        HomeOwnerName = message.Model.NewHomeownerName,
                        HomePhone = message.Model.NewHomeownerHomePhone,
                        OtherPhone = message.Model.NewHomeownerOtherPhone,
                        WorkPhone1 = message.Model.NewHomeownerWorkPhone1,
                        WorkPhone2 = message.Model.NewHomeownerWorkPhone2,
                        EmailAddress = message.Model.NewHomeownerEmailAddress,
                    };

                var job = _database.SingleById<Job>(message.Model.JobId);
                job.CurrentHomeOwnerId = newHomeowner.HomeOwnerId;

                _database.Update(job);
                _database.Insert(newHomeowner);
            }
        }
    }
}