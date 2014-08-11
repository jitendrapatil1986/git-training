using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using IBM.Data.DB2.iSeries;

namespace Warranty.JdeImport.Importers
{
    public abstract class ImporterTemplate
    {
        public abstract string ItemName { get; }

        public abstract string SelectionQuery { get; }

        public abstract string DestinationTable { get; }

        public abstract int BatchSize { get; }

        public abstract int NotifyCount { get; }

        public abstract List<KeyValuePair<string, string>> ColumnMappings { get; }

        public void Import()
        {
            Console.Out.WriteLine("\n{0}: Start: {1}", ItemName, DateTime.Now);
            var stopWatch = Stopwatch.StartNew();

            var jdeConnectionString = ConfigurationManager.ConnectionStrings["JDE"].ConnectionString;
            using (var conn = new iDB2Connection(jdeConnectionString))
            {
                conn.Open();
                using (var cmd = new iDB2Command { Connection = conn, CommandText = SelectionQuery })
                {
                    using (var rd = cmd.ExecuteReader())
                    {
                        BulkImport(rd);
                    }
                }
            }

            stopWatch.Stop();
            Console.Out.WriteLine("\n{0}: End: {1}", ItemName, DateTime.Now);
            Console.WriteLine("{0}: Time: {1}\n", ItemName, stopWatch.Elapsed);
        }

        protected virtual void BulkImport(iDB2DataReader rd)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["WarrantyDB"].ConnectionString;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var bulk = new SqlBulkCopy(conn) { BatchSize = BatchSize, BulkCopyTimeout = 60, NotifyAfter = NotifyCount })
                {
                    bulk.DestinationTableName = DestinationTable;
                    ColumnMappings.ForEach(cm => bulk.ColumnMappings.Add(cm.Key, cm.Value));
                    bulk.SqlRowsCopied += (sender, args) => Console.Out.Write("\r{0} rows copied", args.RowsCopied);
                    bulk.WriteToServer(rd);
                }
            }
        }
    }
}
