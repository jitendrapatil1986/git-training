namespace Warranty.Server.IntegrationTests.SetUp
{
    using System;
    using Core.Entities;
    using NPoco;

    public class ProjectEntityBuilder : EntityBuilder<Project>
    {
        public ProjectEntityBuilder(IDatabase database)
            : base(database)
        {
        }

        public override Project GetSaved(Action<Project> action)
        {
            var entity = new Project
                             {
                                 ProjectNumber = "123",
                                 CreatedBy = "test",
                                 CreatedDate = DateTime.UtcNow,
                             };

            return Saved(entity, action);
        }
    }
}