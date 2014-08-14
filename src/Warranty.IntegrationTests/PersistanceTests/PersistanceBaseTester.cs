using NPoco;
using NUnit.Framework;
using Warranty.Core.DataAccess;
using Warranty.Core.Security;
using Warranty.IntegrationTests.Security;

namespace Warranty.IntegrationTests.PersistanceTests
{
    [TestFixture]
    public abstract class PersistanceBaseTester
    {
        public IDatabase TestDatabase;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            DbFactory.Setup(new WarrantyUserSession());
            TestDatabase = DbFactory.DatabaseFactory.GetDatabase();
        }
    }
}