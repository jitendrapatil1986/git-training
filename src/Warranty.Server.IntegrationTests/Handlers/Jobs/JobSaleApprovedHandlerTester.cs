//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Xml.Linq;
//using NUnit.Framework;
//using Should;
//using TIPS.Events.JobEvents;
//using TIPS.Events.Models;
//using Warranty.Core.Entities;
//using Warranty.Core.Enumerations;
//using Job = Warranty.Core.Entities.Job;

//namespace Warranty.Server.IntegrationTests.Handlers.Jobs
//{
//    [TestFixture]
//    public class JobSaleApprovedHandlerTester : BusHandlerTesterBase<JobSaleApproved>
//    {
//        private readonly string _jobNumber = "12345";
//        private readonly string _primaryEmail = "test2@dwhomes.com";
//        private readonly string _primaryPhone = "123456789";

//        public void SendMessage(Community community, Action<JobSaleApproved> modifyMessage)
//        {
//            var sale = new Sale
//            {
//                JobNumber = _jobNumber
//            };

//            var opportunity = new Opportunity
//            {
//                Contact = new Contact
//                {
//                    PhoneNumbers = new List<PhoneNumber>
//                    {
//                        new PhoneNumber {IsPrimary = true, Number = _primaryPhone},
//                        new PhoneNumber {IsPrimary = false, Number = "987654321"}
//                    },
//                    Emails = new List<Email>
//                    {
//                        new Email {Address = "test1@dwhomes.com", IsPrimary = false},
//                        new Email {Address = _primaryEmail, IsPrimary = true}
//                    },
//                    FirstName = "TestFirst",
//                    LastName = "TestLast"
//                }
//            };
//            if (community != null)
//            {
//                opportunity.CommunityNumber = community.CommunityNumber;
//                sale.CommunityNumber = community.CommunityNumber;
//            }
//            var message = new JobSaleApproved
//            {
//                Sale = sale,
//                Opportunity = opportunity
//            };
//            modifyMessage(message);
//            Send(message);
//        }

//        [Test]
//        public void Job_Should_Be_Added()
//        {
//            var community = GetSaved<Community>();

//            SendMessage(community, x => { });

//            using (TestDatabase)
//            {
//                var job = TestDatabase.FetchBy<Job>(sql => sql.Where(j => j.JobNumber == _jobNumber)).Single();
//                job.ShouldNotBeNull();
//                job.JobNumber.ShouldEqual(_jobNumber);
//            }
//        }

//        [Test]
//        public void Invalid_Message_No_Community()
//        {
//            var community = GetSaved<Community>();
//            Assert.Throws<ArgumentException>(() => { SendMessage(null, x => { }); });
//        }

//        [Test]
//        public void Invalid_Message_JobNumber_Is_Null()
//        {
//            var community = GetSaved<Community>();
//            Assert.Throws<ArgumentException>(() => { SendMessage(community, x => { x.Sale.JobNumber = null; }); });
//        }

//        [Test]
//        public void Homeowner_Should_Be_Added()
//        {
//            var builder = GetSaved<Employee>();
//            var salesman = GetSaved<Employee>();
//            var community = GetSaved<Community>();

//            SendMessage(community, x =>
//            {
//                x.Sale.BuilderEmployeeID = int.Parse(builder.Number);
//                x.Sale.SalesConsultantEmployeeID = int.Parse(salesman.Number);
//            });

//            var job = TestDatabase.FetchBy<Job>(sql => sql.Where(j => j.JobNumber == Event.Sale.JobNumber)).Single();
//            var homeOwnerList = TestDatabase.FetchBy<HomeOwner>(sql => sql.Where(h => h.JobId == job.JobId));

//            homeOwnerList.Count.ShouldEqual(1);
//            var homeOwner = homeOwnerList[0];
//            homeOwner.ShouldNotBeNull();
//            homeOwner.EmailAddress.ShouldEqual(_primaryEmail);
//            homeOwner.HomePhone.ShouldEqual(_primaryPhone);
//            homeOwner.JobId.ShouldEqual(job.JobId);
//            homeOwner.HomeOwnerNumber.ShouldEqual(1);

//            job.CurrentHomeOwnerId.ShouldEqual(homeOwner.HomeOwnerId);
//            job.SalesConsultantEmployeeId = salesman.EmployeeId;
//            job.BuilderEmployeeId = builder.EmployeeId;
//        }

//        [Test]
//        public void Homeowner_Should_Replace_Old_Homeowner()
//        {
//            var community = GetSaved<Community>();

//            SendMessage(community, x =>
//            {
//                x.Opportunity.Contact.FirstName = "HomeOwner2";
//                x.Opportunity.Contact.LastName = "HomeOwner2";
//            });
//            var job = TestDatabase.FetchBy<Job>(sql => sql.Where(j => j.JobNumber == Event.Sale.JobNumber)).Single();
//            var homeOwnerList = TestDatabase.FetchBy<HomeOwner>(sql => sql.Where(h => h.JobId == job.JobId));

//            var homeOwner = homeOwnerList[0];
//            homeOwner.ShouldNotBeNull();
//            homeOwner.HomeOwnerNumber.ShouldEqual(1);
//            homeOwner.HomeOwnerName.ShouldEqual("HomeOwner2, HomeOwner2");
//        }

//        private Job GetTestJob(string jobNumber, Community community, Employee builder, Employee salesman)
//        {
//            return GetSaved<Job>(x =>
//            {
//                x.JobId = Guid.Empty;
//                x.JobNumber = jobNumber;
//                x.CloseDate = DateTime.MaxValue;
//                x.AddressLine = "1234 testing lane";
//                x.City = "Test town";
//                x.StateCode = "TX";
//                x.PostalCode = "77477";
//                x.LegalDescription = "This is a legal description test";
//                x.CommunityId = community.CommunityId;
//                x.CurrentHomeOwnerId = null;
//                x.PlanType = "S";
//                x.PlanTypeDescription = "Nothing";
//                x.PlanName = "The Tester";
//                x.PlanNumber = "400";
//                x.Elevation = "A";
//                x.Swing = "West";
//                x.Stage = 7;
//                x.BuilderEmployeeId = builder.EmployeeId;
//                x.SalesConsultantEmployeeId = salesman.EmployeeId;
//                x.WarrantyExpirationDate = DateTime.Now.AddYears(10);
//                x.DoNotContact = false;
//                x.CreatedDate = DateTime.Now.AddDays(-1);
//                x.CreatedBy = "testing";
//                x.UpdatedDate = DateTime.Now.AddDays(-1);
//                x.UpdatedBy = "Test updater";
//            });
//        }

//        private Sale GetTestSale(String jobNumber, Community community, Employee builder, Employee salesman)
//        {
//            return new Sale
//            {
//                JobNumber = jobNumber,
//                JobType = "M",
//                CloseDate = DateTime.Today,
//                CommunityNumber = community.CommunityNumber,
//                BuilderEmployeeID = Int32.Parse(builder.Number),
//                SalesConsultantEmployeeID = Int32.Parse(salesman.Number),
//                Stage = 8,
//                AddressCity = "New Test City",
//                LegalDescription = null,
//                Swing = "North",
//                PlanName = "The New Tester",
//                CommunityName = community.CommunityName,
//                AddressStateAbbreviation = "TX",
//                AddressZipCode = "77477",
//                PlanNumber = "1234",
//                Elevation = "C",
//                AddressLine1 = "911 Testing Terrace"
//            };   
//        }

//        private void Assert_Job_And_Sale_Are_Equal(Job job, Sale sale)
//        {
//            var community = TestDatabase.FetchBy<Community>(x => x.Where(y => y.CommunityNumber == sale.CommunityNumber)).First();
//            var builder = TestDatabase.FetchBy<Employee>(x => x.Where(y => y.Number == sale.BuilderEmployeeID.ToString())).First();
//            var salesman = TestDatabase.FetchBy<Employee>(x => x.Where(y => y.Number == sale.SalesConsultantEmployeeID.ToString())).First();

//            job.JobNumber.ShouldEqual(sale.JobNumber);
//            job.PlanNumber.ShouldEqual(sale.PlanNumber);
//            job.Elevation.ShouldEqual(sale.Elevation);
//            job.AddressLine.ShouldEqual(sale.AddressLine1);
//            job.City.ShouldEqual(sale.AddressCity);
//            job.StateCode.ShouldEqual(sale.AddressStateAbbreviation);
//            job.PostalCode.ShouldEqual(sale.AddressZipCode);
//            job.PlanType.ShouldEqual(sale.JobType);
//            job.CloseDate.ShouldEqual(sale.CloseDate);
//            job.CreatedBy.ShouldEqual("Warranty.Server");
//            job.JdeIdentifier.ShouldEqual(sale.JobNumber);
//            job.PlanName.ShouldEqual(sale.PlanName);
//            job.Swing.ShouldEqual(sale.Swing);
//            if (sale.LegalDescription == null)
//            {
//                job.LegalDescription.ShouldBeNull();
//            }
//            else
//            {
//                job.LegalDescription.ShouldEqual(sale.LegalDescription.ToString());
//            }
//            job.CommunityId.ShouldEqual(community.CommunityId);
//            job.BuilderEmployeeId.ShouldEqual(builder.EmployeeId);
//            job.SalesConsultantEmployeeId.ShouldEqual(salesman.EmployeeId);
//            job.Stage.ShouldEqual(sale.Stage.Value);
//            job.WarrantyExpirationDate.ShouldEqual(sale.CloseDate.Value.AddYears(10));
//        }

//        [Test]
//        public void Job_Should_Be_Updated_Set_LegalDescriptionToNull()
//        {
//            var community = GetSaved<Community>();
//            var oldBuilder = GetSaved<Employee>(x => x.Number = "1");
//            var oldSalesman = GetSaved<Employee>(x => x.Number = "2");

//            var newBuilder = GetSaved<Employee>(x => x.Number = "3");
//            var newSalesman = GetSaved<Employee>(x => x.Number = "4");
//            var jobNumber = "110102";
//            var job = GetTestJob(jobNumber, community, oldBuilder, oldSalesman);
//            var sale = GetTestSale(jobNumber, community, newBuilder, newSalesman);

//            SendMessage(community, x =>
//            {
//                x.Sale = sale;
//            });

//            var jobFromDb = TestDatabase.FetchBy<Job>(sql => sql.Where(j => j.JobNumber == jobNumber)).Single();
//            Assert_Job_And_Sale_Are_Equal(jobFromDb, sale);
//        }

//        [Test]
//        public void Job_Should_Be_Updated_Set_LegalDescriptionToNewDescription()
//        {
//            var community = GetSaved<Community>();
//            var oldBuilder = GetSaved<Employee>(x => x.Number = "11");
//            var oldSalesman = GetSaved<Employee>(x => x.Number = "22");

//            var newBuilder = GetSaved<Employee>(x => x.Number = "33");
//            var newSalesman = GetSaved<Employee>(x => x.Number = "44");

//            var jobNumber = "110110";
//            var job = GetTestJob(jobNumber, community, oldBuilder, oldSalesman);
//            var sale = GetTestSale(jobNumber, community, newBuilder, newSalesman);
//            sale.LegalDescription = new LegalDescription
//            {
//                Block = "testBlock",
//                Lot = "testLot",
//                Phase = "testPhase",
//                Section = "testSection"
//            };
//            SendMessage(community, x =>
//            {
//                x.Sale = sale;
//            });

//            var jobFromDb = TestDatabase.FetchBy<Job>(sql => sql.Where(j => j.JobNumber == jobNumber)).Single();
//            Assert_Job_And_Sale_Are_Equal(jobFromDb, sale);
//        }

//        private void Should_Create_Todos(int stage, TaskType taskType)
//        {
//            var community = GetSaved<Community>();
//            var wsr = GetSaved<Employee>();
//            using (TestDatabase)
//            {
//                var communityAssignment = new CommunityAssignment
//                {
//                    CommunityId = community.CommunityId,
//                    EmployeeId = wsr.EmployeeId
//                };
//                TestDatabase.Insert(communityAssignment);
//            }

//            SendMessage(community, x => { x.Sale.Stage = stage; });

//            using (TestDatabase)
//            {
//                var job = TestDatabase.FetchBy<Job>(sql => sql.Where(j => j.JobNumber == _jobNumber)).Single();
//                var allTasks = GetTasks(job.JobId, taskType);
//                allTasks.Count.ShouldEqual(1);
//                allTasks.First().TaskType.ShouldEqual(taskType);
//            }
//        }

//        [Test]
//        public void Job_Should_Create_ToDos_Stage3()
//        {
//            Should_Create_Todos(3, TaskType.JobStage3);
//        }

//        [Test]
//        public void Job_Should_Create_ToDos_Stage7()
//        {
//            Should_Create_Todos(7, TaskType.JobStage7);
//        }

//        [Test]
//        public void Job_Should_Create_ToDos_Stage10()
//        {
//            Should_Create_Todos(10, TaskType.JobStage10JobClosed);
//        }

//        private List<Task> GetTasks(Guid jobId, TaskType taskType)
//        {
//            return TestDatabase.Fetch<Task>(string.Format("WHERE ReferenceId = '{0}' and TaskType = {1}", jobId, taskType.Value));
//        }
//    }
//}