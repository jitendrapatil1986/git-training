namespace Warranty.Server.IntegrationTests.Handlers.Jobs
{
    using System.Linq;
    using Accounting.Events;
    using Core.Entities;
    using NUnit.Framework;
    using Should;
    using CommunityAdded = Accounting.Events.Community.CommunityAdded;

    [TestFixture]
    public class CommunityAddedHandlerTester : HandlerTester<CommunityAdded>
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            var division = GetSaved<Division>();
            var project = GetSaved<Project>();
            var city = GetSaved<City>();

            Send(x =>
                     {
                         x.Community = new JdeEventField {JDEId = "43210000", Name = "CommunityName"};
                         x.Division = new JdeEventField {JDEId = division.DivisionCode, Name = "DivisionName"};
                         x.Project = new JdeEventField {JDEId = project.ProjectNumber, Name = "ProjectName"};
                         x.ProductType = new JdeEventField {JDEId = "WKY", Name = "ProductType"};
                         x.Market = new JdeEventField {JDEId = city.CityCode, Name = "INDIANAPOLIS"};
                         x.Status = new JdeEventField {JDEId = "A", Name = "Acitve Community"};
                     });
        }

        [Test]
        public void Community_Should_Be_Added()
        {
            using (TestDatabase)
            {
                var community = TestDatabase.FetchBy<Community>(sql => sql.Where(j => j.CommunityNumber == Event.Community.JDEId.Substring(0, 4))).Single();
                community.ShouldNotBeNull();
            }
        }
    }
}