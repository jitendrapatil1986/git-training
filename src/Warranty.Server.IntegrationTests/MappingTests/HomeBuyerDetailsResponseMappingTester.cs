using System;
using System.Collections.Generic;
using AutoMapper;
using NUnit.Framework;
using Should;
using TIPS.Commands.Responses;
using TIPS.Events.Models;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.MappingTests
{
    [TestFixture]
    public class HomeBuyerDetailsResponseMappingTester : MappingTest
    {
        [Test]
        public void ShouldMapEmptyReponse()
        {
            var response = new HomeBuyerDetailsResponse();
            var homeBuyer = Mapper.Map<HomeOwner>(response);
            homeBuyer.ShouldNotBeNull();
            homeBuyer.HomeOwnerName.ShouldBeNull();
        }

        [Test]
        public void ShouldMapFields()
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
            var homeBuyer = Mapper.Map<HomeOwner>(response);
            homeBuyer.ShouldNotBeNull();
            homeBuyer.HomeOwnerId.ShouldEqual(Guid.Empty);
            homeBuyer.JobId.ShouldBeNull();

            homeBuyer.HomeOwnerName.ShouldEqual("Smith, John");
            homeBuyer.HomePhone.ShouldEqual("111-111-1111");
            homeBuyer.EmailAddress.ShouldEqual("primary@email.com");
        }
    }
}