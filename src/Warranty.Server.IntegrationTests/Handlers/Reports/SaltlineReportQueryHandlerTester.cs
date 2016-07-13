﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Common.Security.Session;
using Moq;
using NUnit.Framework;
using Should;
using Warranty.Core;
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
        private Employee _employee;

        [TestFixtureSetUp]
        public void Setup()
        {
            var city = GetSaved<City>(c => { c.CityCode = "HOU"; });
            var project = GetSaved<Project>(p => { p.ProjectName = "Project1"; });
            var division = GetSaved<Division>(d => { d.DivisionName = "Division1"; }); 
            _employee = GetSaved<Employee>();

            var communities = AddEmployeeToCommunities(city, project, division, _employee);
            var jobs = AddJobs(communities);

            AddHomeOwners(jobs);
            AddServiceCalls(_employee, jobs);
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
            var result = new SaltlineReportModel();

            SystemTime.Stub(new DateTime(2016, 4, 30), () =>
            {
                result = _saltlineReportQueryHandler.Handle(new SaltlineReportQuery
                {
                    queryModel = new SaltlineReportModel
                    {
                        StartDate = DateTime.Parse("04/01/2016"),
                        EndDate = DateTime.Parse("04/30/2016")
                    }
                });
            });

            var employeeSaltlineSummary = result.EmployeeSaltlineSummary.Single();

            decimal.Round(employeeSaltlineSummary.AverageDaysServiceCallsOpen, 1).ShouldEqual(79.3m);
            employeeSaltlineSummary.NumberOfHomes.ShouldEqual(4);
            employeeSaltlineSummary.NumerOfCalls.ShouldEqual(24);
        }

        [Test]
        public void ShouldReturnCorrectValuesForProjectAverageDaysSection()
        {
            var result = new SaltlineReportModel();

            SystemTime.Stub(new DateTime(2016, 4, 30), () =>
            {
                result = _saltlineReportQueryHandler.Handle(new SaltlineReportQuery
                {
                    queryModel = new SaltlineReportModel
                    {
                        StartDate = DateTime.Parse("04/01/2016"),
                        EndDate = DateTime.Parse("04/30/2016")
                    }
                });
            });

            var projectSaltlineSummary = result.ProjectSaltlineSummary.Single();
            
            decimal.Round(projectSaltlineSummary.AverageDaysServiceCallsOpen, 1).ShouldEqual(79.3m);
            projectSaltlineSummary.NumberOfHomes.ShouldEqual(4);
            projectSaltlineSummary.NumerOfCalls.ShouldEqual(24);
        }

        [Test]
        public void ShouldReturnCorrectValuesForDivisionAverageDaysSection()
        {
            var result = new SaltlineReportModel();

            SystemTime.Stub(new DateTime(2016, 4, 30), () =>
            {
                result = _saltlineReportQueryHandler.Handle(new SaltlineReportQuery
                {
                    queryModel = new SaltlineReportModel
                    {
                        StartDate = DateTime.Parse("04/01/2016"),
                        EndDate = DateTime.Parse("04/30/2016")
                    }
                });
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

        [Test]
        public void ShouldReturnCorrectValuesForEmployeeAverageDaysSectionWhenServiceCallsOpenIncludesSpecialProjects()
        {
            var result = new SaltlineReportModel();

            using (new TransactionScope())
            {
                UpdateSpecialProjectOnServiceCall();

                SystemTime.Stub(new DateTime(2016, 4, 30), () =>
                {
                    result = _saltlineReportQueryHandler.Handle(new SaltlineReportQuery
                    {
                        queryModel = new SaltlineReportModel
                        {
                            StartDate = DateTime.Parse("04/01/2016"),
                            EndDate = DateTime.Parse("04/30/2016")
                        }
                    });
                });
            }

            var employeeSaltlineSummary = result.EmployeeSaltlineSummary.Single();

            decimal.Round(employeeSaltlineSummary.AverageDaysServiceCallsOpen, 1).ShouldEqual(81.5m);
            employeeSaltlineSummary.NumberOfHomes.ShouldEqual(4);
            employeeSaltlineSummary.NumerOfCalls.ShouldEqual(23);
        }

        [Test]
        public void ShouldReturnCorrectValuesForProjectAverageDaysSectionWhenServiceCallsOpenIncludesSpecialProjects()
        {
            var result = new SaltlineReportModel();

            using (new TransactionScope())
            {
                UpdateSpecialProjectOnServiceCall();

                SystemTime.Stub(new DateTime(2016, 4, 30), () =>
                {
                    result = _saltlineReportQueryHandler.Handle(new SaltlineReportQuery
                    {
                        queryModel = new SaltlineReportModel
                        {
                            StartDate = DateTime.Parse("04/01/2016"),
                            EndDate = DateTime.Parse("04/30/2016")
                        }
                    });
                });
            }

            var projectSaltlineSummary = result.ProjectSaltlineSummary.Single();

            decimal.Round(projectSaltlineSummary.AverageDaysServiceCallsOpen, 1).ShouldEqual(81.5m);
            projectSaltlineSummary.NumberOfHomes.ShouldEqual(4);
            projectSaltlineSummary.NumerOfCalls.ShouldEqual(23);
        }

        [Test]
        public void ShouldReturnCorrectValuesForDivisionAverageDaysSectionWhenServiceCallsOpenIncludesSpecialProjects()
        {
            var result = new SaltlineReportModel();

            using (new TransactionScope())
            {
                UpdateSpecialProjectOnServiceCall();

                SystemTime.Stub(new DateTime(2016, 4, 30), () =>
                {
                    result = _saltlineReportQueryHandler.Handle(new SaltlineReportQuery
                    {
                        queryModel = new SaltlineReportModel
                        {
                            StartDate = DateTime.Parse("04/01/2016"),
                            EndDate = DateTime.Parse("04/30/2016")
                        }
                    });
                });
            }

            var divisionSaltlineSummary = result.DivisionSaltlineSummary.Single();

            decimal.Round(divisionSaltlineSummary.AverageDaysServiceCallsOpen, 1).ShouldEqual(81.5m);
            divisionSaltlineSummary.NumberOfHomes.ShouldEqual(4);
            divisionSaltlineSummary.NumerOfCalls.ShouldEqual(23);
        }

        private void UpdateSpecialProjectOnServiceCall()
        {
            TestDatabase.Execute(@"
                    UPDATE TOP (1) dbo.ServiceCalls 
                    SET SpecialProject = 1
                    WHERE WarrantyRepresentativeEmployeeId = @0
                        AND CreatedDate BETWEEN '04/01/2016' AND '04/30/2016';", _employee.EmployeeId);
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