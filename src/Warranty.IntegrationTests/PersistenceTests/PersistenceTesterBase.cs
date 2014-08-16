namespace Warranty.IntegrationTests.PersistenceTests
{
    using System;
    using Core;
    using Core.DataAccess;
    using NPoco;
    using NUnit.Framework;
    using StructureMap;
    using Tests.Core;

    [TestFixture]
    public abstract class PersistenceTesterBase
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            ObjectFactory.Initialize(x => x.AddRegistry<WarrantyCoreRegistry>()); 
            DbFactory.Setup(new TestWarrantyUserSession());

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
