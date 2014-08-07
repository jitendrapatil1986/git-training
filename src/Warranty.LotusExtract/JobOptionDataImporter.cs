namespace Warranty.LotusExtract
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Columns;

    public class JobOptionDataImporter
    {
        private const char _fieldDelimiter = '╫';

        protected static Dictionary<string, int> ColumnIndexLookup = new Dictionary<string, int>();

        public void Import(string fileName, string marketList)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["WarrantyDB"].ConnectionString;

            using (var sc = new SqlConnection(connectionString))
            {
                sc.Open();

                using (var cmd = new SqlCommand("TRUNCATE TABLE imports.JobOptionImports", sc))
                    cmd.ExecuteNonQuery();

                using (var file = new StreamReader(fileName))
                {
                    var row = "";
                    string line;

                    var headerColumnCount = GetColumnIndexes(file);

                    while ((line = file.ReadLine()) != null)
                    {
                        line = line
                            .Replace("\"" + _fieldDelimiter + "\"", _fieldDelimiter.ToString(CultureInfo.InvariantCulture))
                            .TrimStart('"')
                            .TrimEnd('"');

                        row += line;
                        if (row.Split(_fieldDelimiter).Count() >= headerColumnCount)
                        {
                            var items = row.Split(_fieldDelimiter);

                            var sql = @"INSERT INTO imports.JobOptionImports
                                        SELECT @jobNumber, @quantity, @option, @description";

                            using (var cmd = new SqlCommand(sql, sc))
                            {
                                cmd.Parameters.Add(new SqlParameter("@jobNumber", items[JobOptionsColumns.JobNumber]));
                                cmd.Parameters.Add(new SqlParameter("@quantity", items[JobOptionsColumns.Quantity]));
                                cmd.Parameters.Add(new SqlParameter("@option", items[JobOptionsColumns.Option]));
                                cmd.Parameters.Add(new SqlParameter("@description", items[JobOptionsColumns.OptionDescription]));
                                cmd.ExecuteNonQuery();
                            }

                            row = "";
                        }
                        else
                        {
                            row += "\r\n";
                        }
                    }
                }
            }
        }

        private static int GetColumnIndexes(StreamReader file)
        {
            var line = file.ReadLine();
            if (line != null)
            {
                var rowUnquoted = line
                    .Replace("\"" + _fieldDelimiter + "\"", _fieldDelimiter.ToString(CultureInfo.InvariantCulture))
                    .TrimStart('"')
                    .TrimEnd('"');

                return rowUnquoted.Split(_fieldDelimiter).Count();
            }

            return 0;
        }
    }
}