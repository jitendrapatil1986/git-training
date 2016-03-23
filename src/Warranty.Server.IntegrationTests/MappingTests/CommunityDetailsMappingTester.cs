using System;
using AutoMapper;
using Newtonsoft.Json;
using NUnit.Framework;
using Should;
using Warranty.Core.Entities;
using Warranty.Core.Services.Models;

namespace Warranty.Server.IntegrationTests.MappingTests
{
    [TestFixture]
    public class CommunityDetailsMappingTester : MappingTest
    {
        const string sampleJson = @"{""Community"":{""JDEId"":""60010000"",""Name"":""MSP Scattered Lots""},""Division"":{""JDEId"":""MSP"",""Name"":""PETERSON""},""Project"":{""JDEId"":""006"",""Name"":""MINNEAPOLIS - ST.PAUL UNASSIGNED""},""Area"":{""JDEId"":""MOO"",""Name"":""MOORE AREA""},""Market"":{""JDEId"":""MSP"",""Name"":""MINNEAPOLIS - ST.PAUL""},""ConstructionType"":{""JDEId"":"""",""Name"":""Production""},""Company"":{""JDEId"":""00001"",""Name"":""David Weekley Homes - TESTDATA""},""Status"":{""JDEId"":""A"",""Name"":""Active Community""},""State"":{""JDEId"":""MN"",""Name"":""Minnesota""},""ProductType"":{""JDEId"":""WKY"",""Name"":""Weekley Product""},""MarketingName"":null,""CommunityClosed"":""UDC NOT FOUND"",""IsBuilding"":false,""IsActive"":true}";

        public override void Setup()
        {
            base.Setup();

            Response = JsonConvert.DeserializeObject<CommunityDetails>(sampleJson);
        }

        public CommunityDetails Response { get; set; }

        [Test]
        public void ShouldMapEmptyDetails()
        {
            var details = new CommunityDetails();
            var community = Mapper.Map<Community>(details);
            community.ShouldNotBeNull();
        }

        [Test]
        public void ShouldMapFieldsCorrectlyToCommunity()
        {
            // sample JSON from the service
            
            var community = Mapper.Map<Community>(Response);

            community.ShouldNotBeNull();
            community.CommunityId.ShouldEqual(Guid.Empty);
            community.CityId.HasValue.ShouldEqual(false);
            community.DivisionId.HasValue.ShouldEqual(false);
            community.ProjectId.HasValue.ShouldEqual(false);
            community.SateliteCityId.HasValue.ShouldEqual(false);

            community.CommunityName.ShouldEqual("MSP Scattered Lots");
            community.CommunityNumber.ShouldEqual("6001");
            community.CommunityStatusCode.ShouldEqual("A");
            community.CommunityStatusDescription.ShouldEqual("Active Community");
            community.ProductType.ShouldEqual("WKY");
            community.ProductTypeDescription.ShouldEqual("Weekley Product");
        }

        [Test]
        public void ShouldMapFieldsCorrectlyToCity()
        {
            var city = Mapper.Map<City>(Response);
            city.ShouldNotBeNull();
            city.CityId.ShouldEqual(Guid.Empty);
            city.CityCode.ShouldEqual("MSP");
            city.CityName.ShouldEqual("MINNEAPOLIS - ST.PAUL");
        }

        [Test]
        public void ShouldMapFieldsCorrectlyToDivision()
        {
            var division = Mapper.Map<Division>(Response);
            division.ShouldNotBeNull();
            division.DivisionId.ShouldEqual(Guid.Empty);
            division.AreaCode.ShouldEqual("MOO");
            division.AreaName.ShouldEqual("MOORE AREA");
            division.DivisionName.ShouldEqual("PETERSON");
            division.DivisionCode.ShouldEqual("MSP");
        }

        [Test]
        public void ShouldMapFieldsCorrectlyToProject()
        {
            var project = Mapper.Map<Project>(Response);
            project.ShouldNotBeNull();
            project.ProjectId.ShouldEqual(Guid.Empty);
            project.ProjectName.ShouldEqual("MINNEAPOLIS - ST.PAUL UNASSIGNED");
            project.ProjectNumber.ShouldEqual("006");
        }
    }
}