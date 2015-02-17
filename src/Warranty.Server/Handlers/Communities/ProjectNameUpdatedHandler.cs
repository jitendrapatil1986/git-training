namespace Warranty.Server.Handlers.Communities
{
    using Accounting.Events;
    using Core.Entities;
    using NPoco;
    using NServiceBus;

    public class ProjectNameUpdatedHandler : IHandleMessages<ProjectNameUpdated>
    {
        private readonly IDatabase _database;

        public ProjectNameUpdatedHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(ProjectNameUpdated message)
        {
            var project = _database.SingleOrDefault<Project>("WHERE ProjectNumber=@0", message.JDEId);
            if (project == null)
                return;

            project.ProjectName = message.Name;
            _database.Save<Project>(project);
        }
    }
}