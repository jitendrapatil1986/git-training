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
    public class JobSaleApprovedHandlerTester : BusHandlerTesterBase<JobSaleApproved>
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

            SendMessage(community, x => { });

            using (TestDatabase)
            {
                var job = TestDatabase.FetchBy<Job>(sql => sql.Where(j => j.JobNumber == _jobNumber)).Single();
                job.ShouldNotBeNull();
                job.JobNumber.ShouldEqual(_jobNumber);
            }
        }

        [Test]
        public void Invalid_Message_No_Community()
        {
            var community = GetSaved<Community>();
            Assert.Throws<ArgumentException>(() => { SendMessage(null, x => { }); });
        }

        [Test]
        public void Invalid_Message_JobNumber_Is_Null()
        {
            var community = GetSaved<Community>();
            Assert.Throws<ArgumentException>(() => { SendMessage(community, x => { x.Sale.JobNumber = null; }); });
        }

        [Test]
        public void Homeowner_Should_Be_Added()
        {
            var builder = GetSaved<Employee>();
            var salesman = GetSaved<Employee>();
            var community = GetSaved<Community>();

            SendMessage(community, x =>
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
            homeOwner.HomeOwnerNumber.ShouldEqual(1);

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
            homeOwner.HomeOwnerNumber.ShouldEqual(1);
            homeOwner.HomeOwnerName.ShouldEqual("HomeOwner2, HomeOwner2");
        }

        [Test]
        public void Job_Should_Be_Updated()
        {
            var community = GetSaved<Community>();
            var oldBuilder = GetSaved<Employee>(x => x.Number = "1");
            var oldSalesman = GetSaved<Employee>(x => x.Number = "2");

            var newBuilder = GetSaved<Employee>(x => x.Number = "3");
            var newSalesman = GetSaved<Employee>(x => x.Number = "4");

            var job = GetSaved<Job>(x =>
            {
                x.JobId = Guid.Empty;
                x.JobNumber = "110102";
                x.CloseDate = DateTime.MaxValue;
                x.AddressLine = "1234 testing lane";
                x.City = "Test town";
                x.StateCode = "TX";
                x.PostalCode = "77477";
                x.LegalDescription = "This is a legal description test";
                x.CommunityId = community.CommunityId;
                x.CurrentHomeOwnerId = null;
                x.PlanType = "S";
                x.PlanTypeDescription = "Nothing";
                x.PlanName = "The Tester";
                x.PlanNumber = "400";
                x.Elevation = "A";
                x.Swing = "West";
                x.Stage = 7;
                x.BuilderEmployeeId = oldBuilder.EmployeeId;
                x.SalesConsultantEmployeeId = oldSalesman.EmployeeId;
                x.WarrantyExpirationDate = DateTime.Now.AddYears(10);
                x.DoNotContact = false;
                x.CreatedDate = DateTime.Now.AddDays(-1);
                x.CreatedBy = "testing";
                x.UpdatedDate = DateTime.Now.AddDays(-1);
                x.UpdatedBy = "Test updater";
            });
            var sale = new Sale
            {
                JobNumber = "110102",
                JobType = "M",
                CloseDate = DateTime.Today,
                CommunityNumber = community.CommunityNumber,
                BuilderEmployeeID = Int32.Parse(newBuilder.Number),
                SalesConsultantEmployeeID = Int32.Parse(newSalesman.Number),
                Stage = 8,
                AddressCity = "New Test City",
                LegalDescription = null,
                Swing = "North",
                PlanName = "The New Tester",
                CommunityName = community.CommunityName,
                AddressStateAbbreviation = "TX",
                AddressZipCode = "77477",
                PlanNumber = "1234",
                Elevation = "C",
                AddressLine1 = "911 Testing Terrace"
            };
            SendMessage(community, x =>
            {
                x.Sale = sale;
            });

            var jobFromDb = TestDatabase.FetchBy<Job>(sql => sql.Where(j => j.JobNumber == "110102")).Single();

            jobFromDb.JobNumber.ShouldEqual(sale.JobNumber);
            jobFromDb.PlanNumber.ShouldEqual(sale.PlanNumber);
            jobFromDb.Elevation.ShouldEqual(sale.Elevation);
            jobFromDb.AddressLine.ShouldEqual(sale.AddressLine1);
            jobFromDb.City.ShouldEqual(sale.AddressCity);
            jobFromDb.StateCode.ShouldEqual(sale.AddressStateAbbreviation);
            jobFromDb.PostalCode.ShouldEqual(sale.AddressZipCode);
            jobFromDb.PlanType.ShouldEqual(sale.JobType);
            jobFromDb.CloseDate.ShouldEqual(sale.CloseDate);
            jobFromDb.CreatedBy.ShouldEqual("Warranty.Server");
            jobFromDb.JdeIdentifier.ShouldEqual(sale.JobNumber);
            jobFromDb.PlanName.ShouldEqual(sale.PlanName);
            jobFromDb.Swing.ShouldEqual(sale.Swing);
            if (sale.LegalDescription == null)
            {
                jobFromDb.LegalDescription.ShouldBeNull();
            }
            else
            {
                jobFromDb.LegalDescription.ShouldEqual(sale.LegalDescription.ToString());
            }
            jobFromDb.CommunityId.ShouldEqual(community.CommunityId);
            jobFromDb.BuilderEmployeeId.ShouldEqual(newBuilder.EmployeeId);
            jobFromDb.SalesConsultantEmployeeId.ShouldEqual(newSalesman.EmployeeId);
            jobFromDb.Stage.ShouldEqual(sale.Stage.Value);
            jobFromDb.WarrantyExpirationDate.ShouldEqual(sale.CloseDate.Value.AddYears(10));
        }
    }
}