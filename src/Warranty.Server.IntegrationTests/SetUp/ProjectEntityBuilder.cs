using System.Security.Cryptography;
using Warranty.Server.IntegrationTests.Handlers;

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
            var randomStringGenerator = new RandomStringGenerator();

            var entity = new Project
                             {
                                 ProjectNumber = randomStringGenerator.Get(3),
                                 CreatedBy = "test",
                                 CreatedDate = DateTime.UtcNow,
                             };

            return Saved(entity, action);
        }
    }
}