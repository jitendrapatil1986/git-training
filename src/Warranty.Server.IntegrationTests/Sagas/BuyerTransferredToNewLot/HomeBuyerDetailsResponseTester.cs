﻿using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Moq;
using NUnit.Framework;
using Should;
using TIPS.Commands.Responses;
using TIPS.Events.Models;
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

        [TestFixtureSetUp]
        public void Setup()
        {
            Mapper.Initialize(m => m.AddProfile(new MappingProfile()));

            var jobService = new Mock<IJobService>();
            var homeOwnerService = new Mock<IHomeOwnerService>();
            var taskService = new Mock<ITaskService>();
            var employeeService = new Mock<IEmployeeService>();
            var communityService = new Mock<ICommunityService>();

            SagaData = new BuyerTransferredToNewLotSagaData();

            Saga = new BuyerTransferredToNewLotSaga(jobService.Object, homeOwnerService.Object, taskService.Object, employeeService.Object, communityService.Object)
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
            SagaData.HomeOwner.HomeOwnerName.ShouldEqual("Smith, John");
            SagaData.HomeOwner.HomePhone.ShouldEqual("111-111-1111");
        }

        [Test]
        public void ShouldSendToEnsureJobExists()
        {
            SagaData.NewJobNumber = "12345";

            var response = new HomeBuyerDetailsResponse();
            Saga.Handle(response);

            var sentMessage = Bus.SentLocalMessages.OfType<BuyerTransferredToNewLotSaga_EnsureNewJobExists>().FirstOrDefault(m => m.NewJobNumber == "12345");
            sentMessage.ShouldNotBeNull();
        }

        
    }
}