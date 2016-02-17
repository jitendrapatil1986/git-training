using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Should;
using StructureMap;
using Warranty.Core.Services;

namespace Warranty.Server.IntegrationTests.Services
{
    [TestFixture]
    public class CommunityServiceTester : ServiceTesterBase
    {
        private ICommunityService _communityService;

        public CommunityServiceTester()
        {
            _communityService = ObjectFactory.GetInstance<ICommunityService>();
        }

        [TestCase("", "0000")]
        [TestCase("1234", "1234")]
        [TestCase("12340000", "1234")]
        [TestCase("0023123", "0023")]
        [TestCase("12", "1200")]
        public void Check_Community_Number_Truncation(string communityNumber, string expectedWarrantyCommunityNumber)
        {
            expectedWarrantyCommunityNumber.ShouldEqual(_communityService.GetWarrantyCommunityNumber(communityNumber));
        }
    }
}
