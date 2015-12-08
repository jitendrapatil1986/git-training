using System;
using System.Collections.Generic;
using Moq;
using NPoco;
using NServiceBus;
using NUnit.Framework;
using Warranty.Core.Entities;
using Warranty.Core.Features.AssignWSRs;

namespace Warranty.Tests.Core.Features.AssignWSRs
{
    [TestFixture]
    public class WhenExistingAssignmentAndThereAreNoTasksAssigned
    {
        /*[Test]
        public void ShouldNotTryToUpdateExistingTasks()
        {
            var database = new Mock<IDatabase>(MockBehavior.Strict);
            database.Setup(db => db.FirstOrDefault<CommunityAssignment>(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(new CommunityAssignment());
            database.Setup(db => db.Single<string>(It.IsAny<string>(), It.IsAny<object[]>())).Returns("1234");
            database.Setup(db => db.Fetch<Task>(It.IsAny<string>(), It.IsAny<object[]>())).Returns(new List<Task>());
            database.Setup(db => db.Dispose());
            var bus = new Mock<IBus>();
            var handler = new AssignWSRCommandHandler(database.Object, bus.Object);
            handler.Handle(new AssignWSRCommand
            {
                CommunityId = Guid.NewGuid(),
                EmployeeId = Guid.NewGuid()
            });
        }*/
    }
}
