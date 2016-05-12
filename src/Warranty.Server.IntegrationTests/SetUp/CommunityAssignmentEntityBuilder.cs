using System;
using NPoco;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.SetUp
{
    public class CommunityAssignmentEntityBuilder : EntityBuilder<CommunityAssignment>
    {
        public CommunityAssignmentEntityBuilder(IDatabase database) : base(database)
        {
        }

        public override CommunityAssignment GetSaved(Action<CommunityAssignment> action)
        {
            var entity = new CommunityAssignment();
            return Saved(entity, action);
        }
    }
}