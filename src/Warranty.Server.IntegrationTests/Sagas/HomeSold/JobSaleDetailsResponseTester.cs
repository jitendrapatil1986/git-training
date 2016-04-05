using System;
using System.Linq;
using log4net;
using Moq;
using NUnit.Framework;
using Should;
using TIPS.Commands.Responses;
using Warranty.Core.Entities;
using Warranty.Core.Services;
using Warranty.Server.Handlers.Jobs;
using Warranty.Server.Sagas;

namespace Warranty.Server.IntegrationTests.Sagas.HomeSold
{
    public class JobSaleDetailsResponseTester : UseDummyBus
    {
        private const string Community_Exists = "2387228378";
        private const string Community_DoesNotExist = "47378382";

        [TestFixtureSetUp]
        public void Setup()
        {
            var jobService = new Mock<IJobService>();
            var homeOwnerService = new Mock<IHomeOwnerService>();
            var taskService = new Mock<ITaskService>();
            var employeeService = new Mock<IEmployeeService>();
            Log = new Mock<ILog>();

            Log.Setup(m => m.ErrorFormat(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<object>())).Verifiable();

            CommunityService = new Mock<ICommunityService>();
            CommunityService.Setup(m => m.GetCommunityByNumber(Community_DoesNotExist))
                .Returns(null as Community)
                .Verifiable();

            CommunityService.Setup(m => m.GetCommunityByNumber(Community_Exists))
                .Returns(new Community
                {
                    CommunityId = Guid.NewGuid(),
                    CommunityNumber = Community_Exists
                })
                .Verifiable();

            SagaData = new HomeSoldSagaData();
            Saga = new HomeSoldSaga(CommunityService.Object, jobService.Object, employeeService.Object, homeOwnerService.Object, taskService.Object, Log.Object)
            {
                Data = SagaData,
                Bus = Bus
            };
        }

        public Mock<ILog> Log { get; set; }

        public Mock<ICommunityService> CommunityService { get; set; }

        public HomeSoldSaga Saga { get; set; }

        public HomeSoldSagaData SagaData { get; set; }

        [Test]
        public void ShouldSetDataFields()
        {
            SagaData.JobSaleDetails = null;
            var message = new JobSaleDetailsResponse
            {
                CommunityNumber = "1287872",
                BuilderEmployeeID = 29832983
            };
            Saga.Handle(message);
            SagaData.JobSaleDetails.ShouldNotBeNull();
        }

        [Test]
        public void ShouldSendToGetCommunityDetailsIfNotFound()
        {
            Log.ResetCalls();
            SagaData.JobSaleDetails = null;
            SagaData.JobNumber = "928192818";
            SagaData.SaleId = 834738473748;
            

            CommunityService.ResetCalls();
            var message = new JobSaleDetailsResponse
            {
                CommunityNumber = Community_DoesNotExist,
                BuilderEmployeeID = 29832983
            };
            Saga.Handle(message);

            CommunityService.Verify(m => m.GetCommunityByNumber(Community_DoesNotExist), Times.Once);
            
            var sentMessage = Bus.SentLocalMessages.OfType<HomeSoldSaga_GetCommunityDetails>().FirstOrDefault(m => m.SaleId == 834738473748);

            // Disabled for now until it is implemented
            sentMessage.ShouldNotBeNull();
        }

        [Test]
        public void ShouldSetDataFieldsWhenCommunityFounds()
        {
            SagaData.JobSaleDetails = null;
            SagaData.JobNumber = "2342456";
            SagaData.CommunityReferenceId = Guid.Empty;
            CommunityService.ResetCalls();

            var message = new JobSaleDetailsResponse
            {
                CommunityNumber = Community_Exists,
                BuilderEmployeeID = 29832983
            };
            Saga.Handle(message);

            CommunityService.Verify(m => m.GetCommunityByNumber(Community_Exists), Times.Once);
            SagaData.CommunityReferenceId.ShouldNotEqual(Guid.Empty);
        }

        [Test]
        public void ShouldSendToCreateOrUpdateJobWhenCommunityDetailsFound()
        {
            SagaData.JobSaleDetails = null;
            SagaData.JobNumber = "88857423";
            SagaData.SaleId = 3923928398;
            CommunityService.ResetCalls();

            var message = new JobSaleDetailsResponse
            {
                CommunityNumber = Community_Exists,
                BuilderEmployeeID = 29832983
            };
            Saga.Handle(message);

            CommunityService.Verify(m => m.GetCommunityByNumber(Community_Exists), Times.Once);

            var sentMessage = Bus.SentLocalMessages.OfType<HomeSoldSaga_CreateOrUpdateJob>().FirstOrDefault(m => m.SaleId == 3923928398);
            sentMessage.ShouldNotBeNull();
        }
    }
}