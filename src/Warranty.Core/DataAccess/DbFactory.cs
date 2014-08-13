namespace Warranty.Core.DataAccess
{
    using NPoco;

    public static class DbFactory
    {
        public static DatabaseFactory DatabaseFactory { get; set; }

        public static void Setup()
        {
            DatabaseFactory = DatabaseFactory.Config(x => x.UsingDatabase(() => new Database("WarrantyDB")));
        }
    }
}
