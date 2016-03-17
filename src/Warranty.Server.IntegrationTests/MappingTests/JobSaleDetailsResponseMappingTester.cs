using AutoMapper;
using NUnit.Framework;
using Should;
using TIPS.Commands.Responses;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.MappingTests
{
    [TestFixture]
    public class JobSaleDetailsResponseMappingTester : MappingTest
    {
        [Test]
        public void ShouldMapEmptyResponse()
        {
            var response = new JobSaleDetailsResponse();
            var job = Mapper.Map<Job>(response);

            job.ShouldNotBeNull();
            job.City.ShouldBeNull();
            job.CreatedBy.ShouldBeNull();
        }

        [Test]
        public void ShouldMapFields()
        {
            var response = new JobSaleDetailsResponse
            {
                Lot = "205",
                Block = "102",
                Section = "405",
                Phase = "223",
                AddressLine1 = "123 Somewhere Rd.",
                AddressCity = "Houston",
                AddressStateAbbreviation = "TX",
                AddressZipCode = "77573",
                JobType = "SomeType",
                JobNumber = "JDE#"
            };
            var job = Mapper.Map<Job>(response);

            job.ShouldNotBeNull();
            job.LegalDescription.ShouldEqual("205/102/405/223");
            job.AddressLine.ShouldEqual("123 Somewhere Rd.");
            job.City.ShouldEqual("Houston");
            job.StateCode.ShouldEqual("TX");
            job.PostalCode.ShouldEqual("77573");
            job.PlanType.ShouldEqual("SomeType");
            job.JdeIdentifier.ShouldEqual("JDE#");
        }
    }
}