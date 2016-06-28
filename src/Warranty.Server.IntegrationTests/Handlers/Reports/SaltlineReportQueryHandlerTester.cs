using System;
using System.Collections.Generic;
using System.Linq;
using Common.Security.Session;
using Moq;
using NUnit.Framework;
using Should;
using Warranty.Core.Calculator;
using Warranty.Core.Entities;
using Warranty.Core.Features.Report.Saltline;
using Warranty.Core.Services;

namespace Warranty.Server.IntegrationTests.Handlers.Reports
{
    [TestFixture]
    public class SaltlineReportQueryHandlerTester : HandlerTester<SaltlineReportQuery>
    {
        private SaltlineReportQueryHandler _saltlineReportQueryHandler;

        [TestFixtureSetUp]
        public void Setup()
        {
            var city = GetSaved<City>(c => { c.CityCode = "HOU"; });
            var project = GetSaved<Project>(p => { p.ProjectName = "Project1"; });
            var division = GetSaved<Division>(d => { d.DivisionName = "Division1"; }); 
            var employee = GetSaved<Employee>();

            var communities = AddEmployeeToCommunities(city, project, division, employee);
            var jobs = AddJobs(communities);

            AddHomeOwners(jobs);
            AddServiceCalls(employee, jobs);
            AddPaymentsToTmpJdeGlWarBuckets(communities);

            var surveyService = new Mock<ISurveyService>();
            var employeeService = new Mock<IEmployeeService>();
            employeeService.Setup(x => x.GetEmployeeMarkets()).Returns("'HOU'");

            var mockUser = new Mock<IUser>();
            mockUser.Setup(x => x.Markets).Returns(new[] { "HOU" });

            var mockUserSession = new Mock<IUserSession>();
            mockUserSession.Setup(x => x.GetCurrentUser()).Returns(mockUser.Object);

            var warrantyCalculator = new WarrantyCalculator(TestDatabase, surveyService.Object, employeeService.Object);

            _saltlineReportQueryHandler = new SaltlineReportQueryHandler(TestDatabase, mockUserSession.Object, warrantyCalculator);
        }

        [Test]
        public void ShouldReturnCorrectValuesForEmployeeAverageDaysSection()
        {
            var result = _saltlineReportQueryHandler.Handle(new SaltlineReportQuery
            {
                queryModel = new SaltlineReportModel
                {
                    StartDate = DateTime.Parse("04/01/2016"),
                    EndDate = DateTime.Parse("04/30/2016")
                }
            });

            var employeeSaltlineSummary = result.EmployeeSaltlineSummary.Single();

            decimal.Round(employeeSaltlineSummary.AverageDaysServiceCallsOpen, 1).ShouldEqual(79.3m);
            employeeSaltlineSummary.NumberOfHomes.ShouldEqual(4);
            employeeSaltlineSummary.NumerOfCalls.ShouldEqual(24);
        }

        [Test]
        public void ShouldReturnCorrectValuesForProjectAverageDaysSection()
        {
            var result = _saltlineReportQueryHandler.Handle(new SaltlineReportQuery
            {
                queryModel = new SaltlineReportModel
                {
                    StartDate = DateTime.Parse("04/01/2016"),
                    EndDate = DateTime.Parse("04/30/2016")
                }
            });

            var projectSaltlineSummary = result.ProjectSaltlineSummary.Single();
            
            decimal.Round(projectSaltlineSummary.AverageDaysServiceCallsOpen, 1).ShouldEqual(79.3m);
            projectSaltlineSummary.NumberOfHomes.ShouldEqual(4);
            projectSaltlineSummary.NumerOfCalls.ShouldEqual(24);
        }

        [Test]
        public void ShouldReturnCorrectValuesForDivisionAverageDaysSection()
        {
            var result = _saltlineReportQueryHandler.Handle(new SaltlineReportQuery
            {
                queryModel = new SaltlineReportModel
                {
                    StartDate = DateTime.Parse("04/01/2016"),
                    EndDate = DateTime.Parse("04/30/2016")
                }
            });

            var divisionSaltline = result.DivisionSaltlineSummary.Single();

            decimal.Round(divisionSaltline.AverageDaysServiceCallsOpen, 1).ShouldEqual(79.3m);
            divisionSaltline.NumberOfHomes.ShouldEqual(4);
            divisionSaltline.NumerOfCalls.ShouldEqual(24);
        }

        [Test]
        public void ShouldReturnCorrectValuesForEmployeeDollarsSpent()
        {
            var result = _saltlineReportQueryHandler.Handle(new SaltlineReportQuery
            {
                queryModel = new SaltlineReportModel
                {
                    StartDate = DateTime.Parse("04/01/2016"),
                    EndDate = DateTime.Parse("04/30/2016")
                }
            });

            var employeeSaltlineSummary = result.EmployeeSaltlineSummary.Single();

            decimal.Round(employeeSaltlineSummary.AmountSpentPerHome, 1).ShouldEqual(400);
        }

        [Test]
        public void ShouldReturnCorrectValuesForProjectDollarsSpent()
        {
            var result = _saltlineReportQueryHandler.Handle(new SaltlineReportQuery
            {
                queryModel = new SaltlineReportModel
                {
                    StartDate = DateTime.Parse("04/01/2016"),
                    EndDate = DateTime.Parse("04/30/2016")
                }
            });

            var projectSaltlineSummary = result.ProjectSaltlineSummary.Single();

            decimal.Round(projectSaltlineSummary.AmountSpentPerHome, 1).ShouldEqual(400);
        }

        [Test]
        public void ShouldReturnCorrectValuesForDivisionDollarsSpent()
        {
            var result = _saltlineReportQueryHandler.Handle(new SaltlineReportQuery
            {
                queryModel = new SaltlineReportModel
                {
                    StartDate = DateTime.Parse("04/01/2016"),
                    EndDate = DateTime.Parse("04/30/2016")
                }
            });

            var divisionSaltlineSummary = result.DivisionSaltlineSummary.Single();

            decimal.Round(divisionSaltlineSummary.AmountSpentPerHome, 1).ShouldEqual(400);
        }

        private void AddPaymentsToTmpJdeGlWarBuckets(IEnumerable<Community> communities)
        {
            foreach (var community in communities)
            {
                GetSaved<Tmp_Jde_Gl_War_Buckets>(warBucket =>
                {
                    warBucket.GBFY = 16;
                    warBucket.GBMCU = community.CommunityNumber + "9999";
                    warBucket.GBOBJ = 9425;
                });

                GetSaved<Tmp_Jde_Gl_War_Buckets>(warBucket =>
                {
                    warBucket.GBFY = 16;
                    warBucket.GBMCU = community.CommunityNumber + "9999";
                    warBucket.GBOBJ = 9430;
                });

                GetSaved<Tmp_Jde_Gl_War_Buckets>(warBucket =>
                {
                    warBucket.GBFY = 16;
                    warBucket.GBMCU = community.CommunityNumber + "9999";
                    warBucket.GBOBJ = 9435;
                });

                GetSaved<Tmp_Jde_Gl_War_Buckets>(warBucket =>
                {
                    warBucket.GBFY = 16;
                    warBucket.GBMCU = community.CommunityNumber + "9999";
                    warBucket.GBOBJ = 9440;
                });
            }
        }

        private IEnumerable<Community> AddEmployeeToCommunities(City city, Project project, Division division, Employee employee)
        {
            var communities = GetManySaved<Community>(2, community =>
            {
                community.CityId = city.CityId;
                community.ProjectId = project.ProjectId;
                community.DivisionId = division.DivisionId;
            });

            var employeeOnecommunityAssignments = communities.Select((community, i) => new CommunityAssignment
            {
                EmployeeAssignmentId = Guid.NewGuid(),
                CommunityId = communities.ElementAt(i).CommunityId,
                EmployeeId = employee.EmployeeId
            });

            TestDatabase.InsertBulk(employeeOnecommunityAssignments);

            return communities;
        }

        private void AddServiceCalls(Employee employee, IEnumerable<Job> jobs)
        {
            foreach (var job in jobs)
            {
                var calls = GetManySaved<ServiceCall>(2).ToList();

                foreach (var call in calls)
                {
                    call.WarrantyRepresentativeEmployeeId = employee.EmployeeId;
                    call.JobId = job.JobId;
                    call.CreatedDate = DateTime.Parse("01/01/2016");

                    InsertServiceCall(call);
                }

                calls = GetManySaved<ServiceCall>(2).ToList();

                foreach (var call in calls)
                {
                    call.WarrantyRepresentativeEmployeeId = employee.EmployeeId;
                    call.JobId = job.JobId;
                    call.CreatedDate = DateTime.Parse("02/01/2016");

                    InsertServiceCall(call);
                }

                calls = GetManySaved<ServiceCall>(2).ToList();

                foreach (var call in calls)
                {
                    call.WarrantyRepresentativeEmployeeId = employee.EmployeeId;
                    call.JobId = job.JobId;
                    call.CreatedDate = DateTime.Parse("04/01/2016");

                    InsertServiceCall(call);
                }

                calls = GetManySaved<ServiceCall>(2).ToList();

                foreach (var call in calls)
                {
                    call.WarrantyRepresentativeEmployeeId = employee.EmployeeId;
                    call.JobId = job.JobId;
                    call.CreatedDate = DateTime.Parse("03/01/2016");
                    call.CompletionDate = DateTime.Parse("03/31/2016");

                    InsertServiceCall(call);
                }
            }
        }

        private void InsertServiceCall(ServiceCall serviceCall)
        {
            TestDatabase.Execute(@"
                INSERT INTO dbo.ServiceCalls 
                (
                    ServiceCallNumber
                    ,ServiceCallType
                    ,ServiceCallStatusId
                    ,JobId
                    ,WarrantyRepresentativeEmployeeId
                    ,CreatedDate
                    ,CompletionDate
                )
                VALUES (@0, @1, @2, @3, @4, @5, @6);", serviceCall.ServiceCallNumber,
                serviceCall.ServiceCallType,
                serviceCall.ServiceCallStatus.Value,
                serviceCall.JobId,
                serviceCall.WarrantyRepresentativeEmployeeId,
                serviceCall.CreatedDate,
                serviceCall.CompletionDate);
        }

        private void AddHomeOwners(IEnumerable<Job> jobs)
        {
            foreach (var job in jobs)
            {
                GetSaved<HomeOwner>(owner =>
                {
                    owner.JobId = job.JobId;
                });
            }
        }

        private IEnumerable<Job> AddJobs(IEnumerable<Community> communities)
        {
            var jobs = new List<Job>();

            foreach (var community in communities)
            {
                var newJobs = GetManySaved<Job>(2, j =>
                {
                    j.CloseDate = DateTime.Parse("04/01/2016");
                    j.CommunityId = community.CommunityId;
                }); 

                jobs.AddRange(newJobs);
            }

            return jobs;
        }
    }
}