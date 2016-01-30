using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using NUnit.Framework;
using Should;
using TIPS.Events.JobEvents;
using TIPS.Events.Models;
using Warranty.Core.Entities;
using Job = Warranty.Core.Entities.Job;

namespace Warranty.Server.IntegrationTests.Handlers.Jobs
{
    [TestFixture]
    public class JobSaleApprovedHandlerTester : IBusHandlerTesterBase<JobSaleApproved>
    {
        private readonly string _jobNumber = "12345";
        private readonly string _primaryEmail = "test2@dwhomes.com";
        private readonly string _primaryPhone = "123456789";

        public void SendMessage(Community community, Action<JobSaleApproved> modifyMessage)
        {
            var sale = new Sale
            {
                JobNumber = _jobNumber
            };

            var opportunity = new Opportunity
            {
                Contact = new Contact
                {
                    PhoneNumbers = new List<PhoneNumber>
                    {
                        new PhoneNumber {IsPrimary = true, Number = _primaryPhone},
                        new PhoneNumber {IsPrimary = false, Number = "987654321"}
                    },
                    Emails = new List<Email>
                    {
                        new Email {Address = "test1@dwhomes.com", IsPrimary = false},
                        new Email {Address = _primaryEmail, IsPrimary = true}
                    },
                    FirstName = "TestFirst",
                    LastName = "TestLast"
                }
            };
            if (community != null)
            {
                opportunity.CommunityNumber = community.CommunityNumber;
                sale.CommunityNumber = community.CommunityNumber;
            }
            var message = new JobSaleApproved
            {
                Sale = sale,
                Opportunity = opportunity
            };
            modifyMessage(message);
            Send(message);
        }

        [Test]
        public void Job_Should_Be_Added()
        {
            var community = GetSaved<Community>();

            SendMessage(community, x => {});
      
            using (TestDatabase)
            {
                var job = TestDatabase.FetchBy<Job>(sql => sql.Where(j => j.JobNumber == Event.Sale.JobNumber)).Single();
                job.ShouldNotBeNull();
                job.JobNumber.ShouldEqual(_jobNumber);
                job.CommunityId.ShouldEqual(community.CommunityId);
            }
        }

        [Test]
        public void Invalid_Message_Information_Test()
        {
            var community = GetSaved<Community>();
            Assert.Throws<ArgumentException>(() =>
            {
                SendMessage(null, x => { });
            });
            Assert.Throws<ArgumentException>(() =>
            {
                SendMessage(community, x =>
                {
                    x.Sale.JobNumber = null;
                });
            });
        }

        [Test]
        public void Homeowner_Should_Be_Added()
        {
            var builder = GetSaved<Employee>();
            var salesman = GetSaved<Employee>();
            var community = GetSaved<Community>();

            SendMessage(community, x=>
            {
                x.Sale.BuilderEmployeeID = int.Parse(builder.Number);
                x.Sale.SalesConsultantEmployeeID = int.Parse(salesman.Number);
            });

            var job = TestDatabase.FetchBy<Job>(sql => sql.Where(j => j.JobNumber == Event.Sale.JobNumber)).Single();
            var homeOwnerList = TestDatabase.FetchBy<HomeOwner>(sql => sql.Where(h => h.JobId == job.JobId));

            homeOwnerList.Count.ShouldEqual(1);
            var homeOwner = homeOwnerList[0];
            homeOwner.ShouldNotBeNull();
            homeOwner.EmailAddress.ShouldEqual(_primaryEmail);
            homeOwner.HomePhone.ShouldEqual(_primaryPhone);
            homeOwner.JobId.ShouldEqual(job.JobId);
            homeOwner.HomeOwnerNumber.ShouldEqual(0);

            job.CurrentHomeOwnerId.ShouldEqual(homeOwner.HomeOwnerId);
            job.SalesConsultantEmployeeId = salesman.EmployeeId;
            job.BuilderEmployeeId = builder.EmployeeId;
        }

        [Test]
        public void Homeowner_Should_Replace_Old_Homeowner()
        {
            var community = GetSaved<Community>();

            SendMessage(community, x =>
            {
                x.Opportunity.Contact.FirstName = "HomeOwner2";
                x.Opportunity.Contact.LastName = "HomeOwner2";
            });
            var job = TestDatabase.FetchBy<Job>(sql => sql.Where(j => j.JobNumber == Event.Sale.JobNumber)).Single();
            var homeOwnerList = TestDatabase.FetchBy<HomeOwner>(sql => sql.Where(h => h.JobId == job.JobId));

            var homeOwner = homeOwnerList[0];
            homeOwner.ShouldNotBeNull();
            homeOwner.HomeOwnerNumber.ShouldEqual(0);
            homeOwner.HomeOwnerName.ShouldEqual("HomeOwner2, HomeOwner2");
        }
    }
}