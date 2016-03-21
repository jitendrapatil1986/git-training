using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Moq;
using NUnit.Framework;
using Should;
using TIPS.Commands.Requests;
using TIPS.Commands.Responses;
using TIPS.Events.Models;
using Warranty.Core.Entities;
using Warranty.Core.Services;
using Warranty.Server.Handlers.Jobs;
using Warranty.Server.Sagas;

namespace Warranty.Server.IntegrationTests.Sagas.BuyerTransferredToNewLot
{
    [TestFixture]
    public class HomeBuyerDetailsResponseTester : UseDummyBus
    {
        public BuyerTransferredToNewLotSagaData SagaData { get; set; }
        public BuyerTransferredToNewLotSaga Saga { get; set; }
        public Mock<IHomeOwnerService> HomeOwnerService { get; set; }

        [TestFixtureSetUp]
        public void Setup()
        {
            Mapper.Initialize(m => m.AddProfile(new MappingProfile()));

            var jobService = new Mock<IJobService>();
            HomeOwnerService = new Mock<IHomeOwnerService>();
            HomeOwnerService.Setup(p => p.Create(It.IsAny<HomeOwner>()))
               .Returns(new HomeOwner())
               .Verifiable();

            var taskService = new Mock<ITaskService>();
            var employeeService = new Mock<IEmployeeService>();
            var communityService = new Mock<ICommunityService>();
            var log = new Mock<log4net.ILog>();

            SagaData = new BuyerTransferredToNewLotSagaData();

            Saga = new BuyerTransferredToNewLotSaga(jobService.Object, HomeOwnerService.Object, taskService.Object, employeeService.Object, communityService.Object, log.Object)
            {
                Bus = Bus,
                Data = SagaData
            };
        }

        [Test]
        public void ShouldSetDataFields()
        {
            var response = new HomeBuyerDetailsResponse
            {
                FirstName = "John",
                LastName = "Smith",
                PhoneNumbers = new List<PhoneNumber>
                {
                    new PhoneNumber { IsPrimary = false, Number = "555-555-5555", Type = "HOME"},
                    new PhoneNumber { IsPrimary = true, Number = "111-111-1111", Type = "WORK"}
                },
                EmailAddresses = new List<Email>
                {
                    new Email { Address = "notprimary@email.com", IsPrimary = false },
                    new Email { Address = "primary@email.com", IsPrimary = true }
                }
            };

            Saga.Handle(response);
            SagaData.HomeOwner.ShouldNotBeNull();
        }

        [Test]
        public void ShouldCreateHomeOwner()
        {
            HomeOwnerService.ResetCalls();
            var response = new HomeBuyerDetailsResponse
            {
                FirstName = "John",
                LastName = "Smith",
                PhoneNumbers = new List<PhoneNumber>
                {
                    new PhoneNumber { IsPrimary = false, Number = "555-555-5555", Type = "HOME"},
                    new PhoneNumber { IsPrimary = true, Number = "111-111-1111", Type = "WORK"}
                },
                EmailAddresses = new List<Email>
                {
                    new Email { Address = "notprimary@email.com", IsPrimary = false },
                    new Email { Address = "primary@email.com", IsPrimary = true }
                }
            };

            Saga.Handle(response);
            HomeOwnerService.Verify(a => a.Create(It.IsAny<HomeOwner>()), Times.Once);
        }

        [Test]
        public void ShouldRequestJobDetails()
        {
            SagaData.SaleId = 12344567;

            var response = new HomeBuyerDetailsResponse();
            Saga.Handle(response);

            var sentMessage = Bus.SentMessages.OfType<RequestJobSaleDetails>().FirstOrDefault(m => m.SaleId == 12344567);
            sentMessage.ShouldNotBeNull();
        }

        
    }
}