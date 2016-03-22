using AutoMapper;
using Newtonsoft.Json;
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

        [Test]
        public void ShouldHandleSampleResponseFromTips()
        {
            // sample values from a response seny by TIPS
            var sampleResponse = @"{
                                   ""SaleId"": ""260855"",
                                   ""ContactId"": ""0"",
                                   ""SaleDate"": ""2015-12-31T00:00:00"",
                                   ""CloseDate"": null,
                                   ""SalesConsultantEmployeeID"": ""9903182"",
                                   ""Lender"": ""FBC Mortgage"",
                                   ""LenderContact"": ""Matt  Andre"",
                                   ""LenderPhone"": ""4073770276"",
                                   ""LoanApplicationDate"": null,
                                   ""LoanApprovalDate"": null,
                                   ""JobNumber"": ""29700042"",
                                   ""JobType"": ""B"",
                                   ""CommunityNumber"": ""29700000"",
                                   ""CommunityName"": ""Spring Lake at Celebration -"",
                                   ""PlanNumber"": ""7324"",
                                   ""PlanName"": ""Cardelle"",
                                   ""Elevation"": ""E"",
                                   ""Swing"": ""S"",
                                   ""AddressLine1"": ""1527 Castile Street"",
                                   ""AddressCity"": ""Celebration"",
                                   ""AddressStateAbbreviation"": ""FL"",
                                   ""AddressZipCode"": ""34747"",
                                   ""AddressCounty"": ""Osceola"",
                                   ""Lot"": ""42"",
                                   ""Block"": """",
                                   ""Section"": """",
                                   ""Phase"": ""02"",
                                   ""FinalSelectionsComplete"": ""true"",
                                   ""StartLockFlag"": ""true"",
                                   ""PrepanelDate"": ""2015-05-05T00:00:00"",
                                   ""StartDate"": ""2015-08-13T00:00:00"",
                                   ""ReadyDate"": ""2016-01-15T00:00:00"",
                                   ""DesignerEmployeeID"": ""9906917"",
                                   ""BuilderEmployeeID"": ""9910255"",
                                   ""Stage"": ""10"",
                                   ""BasePrice"": ""398990"",
                                   ""LotPremium"": ""13265"",
                                   ""CatalogOptionAmount"": ""21986"",
                                   ""HcrAmount"": ""1973"",
                                   ""Incentives"": ""-1000"",
                                   ""Discount"": ""0"",
                                   ""FlexOptions"": ""3535""
                                }";

            var response = JsonConvert.DeserializeObject<JobSaleDetailsResponse>(sampleResponse);
            response.ShouldNotBeNull();

            var job = Mapper.Map<Job>(response);
            job.ShouldNotBeNull();
        }
    }
}