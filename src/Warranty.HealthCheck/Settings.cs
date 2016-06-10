using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Warranty.HealthCheck
{
    public static class Settings
    {
        public static Lazy<string> DatabaseName = new Lazy<string>(GetDatabaseName);

        private static string GetDatabaseName()
        {
            var builder = new SqlConnectionStringBuilder
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["warranty"].ConnectionString
            };
            return builder.InitialCatalog;
        }
    }
}