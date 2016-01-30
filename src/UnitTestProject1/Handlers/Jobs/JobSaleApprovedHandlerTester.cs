
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using FizzWare.NBuilder;
using TIPS.Events;
using NUnit.Framework;
using Should;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NPoco;
using TIPS.Events.JobEvents;
using TIPS.Events.Models;
using Warranty.Core.ActivityLogger;
using Warranty.Core.Entities;
using Warranty.Server.Extensions;
using Warranty.Server.Handlers.Jobs;
using Job = TIPS.Events.Models.Job;

namespace Warranty.Server.Tests.Handlers.Jobs
{
    [TestFixture]
    public class JobSaleApprovedHandlerTester
    {
        Mock<IDatabase> database = new Mock<IDatabase>();
        Mock<IActivityLogger> logger = new Mock<IActivityLogger>();

        private JobSaleApproved GetMessage()
        {
            var sale = Builder<Sale>.CreateNew().Build();
            var opportunity = Builder<Opportunity>.CreateNew().Build();

            return new JobSaleApproved()
            {
                Sale = sale,
                Opportunity = opportunity
            };
        }

        public void Handle(Action setup, Action assertions)
        {
            var ticket = GetMessage();
            setup();
            var jobSaleApprovedHandler = new JobSaleApprovedHandler(database.Object, logger.Object);
            jobSaleApprovedHandler.Handle(ticket);
            assertions();
        }

        [Test]
        public void Test()
        {
            var employeeList = Builder<Employee>.CreateListOfSize(2).Build();
            Queue<Employee> allEmployees = new Queue<Employee>(employeeList);

            Handle(() =>
            {
                database.Setup(x => x.SingleOrDefault<Job>(It.IsAny<string>(), It.IsAny<string>())).Returns(() => null);
                database.Setup(x => x.SingleOrDefault<Community>(It.IsAny<string>(), It.IsAny<string>())).Returns(() => null);
                database.Setup(x => x.SingleOrDefault<Employee>(It.IsAny<string>(), It.IsAny<int?>())).Returns(allEmployees.Dequeue);
            }, () => { });
        }
    }
}
