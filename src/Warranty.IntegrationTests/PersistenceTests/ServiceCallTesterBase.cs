using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warranty.IntegrationTests.PersistenceTests
{
    using Core;
    using Core.DataAccess;
    using NPoco;
    using NUnit.Framework;
    using StructureMap;
    using Tests.Core;

    [TestFixture]
    public abstract class ServiceCallTesterBase
    {
        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            ObjectFactory.Configure(x => x.AddRegistry<WarrantyCoreRegistry>());

            var deleter = new DatabaseDeleter(DbFactory.DatabaseFactory.GetDatabase());
            deleter.DeleteAllData(DbFactory.DatabaseFactory.GetDatabase());
        }

        protected TEntity Insert<TEntity>(TEntity entity)
        {
            using (var db = ObjectFactory.GetInstance<IDatabase>())
            {
                db.Insert(entity);
                return entity;
            }
        }

        protected TEntity Update<TEntity>(TEntity entity)
        {
            using (var db = ObjectFactory.GetInstance<IDatabase>())
            {
                db.Update(entity);
                return entity;
            }
        }

        protected TEntity Load<TEntity>(Guid id)
        {
            using (var db = ObjectFactory.GetInstance<IDatabase>())
            {
                return db.SingleOrDefaultById<TEntity>(id);
            }
        }
    }
}
