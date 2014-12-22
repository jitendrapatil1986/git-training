using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace Warranty.JdeImport.Importers
{
    internal class CommunityCleanUp : ImporterTemplate
    {
        public override string ItemName
        {
            get { return "Communities"; }
        }

        public override string SelectionQuery
        {
            get
            {
                return @"SELECT DISTINCT
                         trim(f1.MCMCU) as CommunityNumber 
                       FROM F0006 f1 
                       INNER JOIN F0101 a ON f1.mcan8 = a.aban8 
                       INNER JOIN F0116 ad on a.aban8 = ad.alan8 
                       WHERE trim(f1.MCSTYL)='TP' AND TRIM(f1.MCMCU) LIKE '%0000'";
            }
        }

        public override string DestinationTable
        {
            get { return "imports.CommunityList"; }
        }

        public override int BatchSize
        {
            get { return 5000; }
        }

        public override int NotifyCount
        {
            get { return 5000; }
        }

        public override List<KeyValuePair<string, string>> ColumnMappings
        {
            get
            {
                return new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("CommunityNumber","CommunityNumber"),
                };
            }
        }

        public void CustomImport()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["WarrantyDB"].ConnectionString;

            const string mergeScript = @"DELETE FROM CommunityAssignments WHERE CommunityId IN (SELECT CommunityId 
                                                        FROM Communities 
                                                        WHERE CommunityId NOT IN (SELECT CommunityId FROM Jobs)
                                                        AND CommunityNumber NOT IN (SELECT CommunityNumber FROM imports.CommunityList));

                                        DELETE FROM Communities WHERE CommunityId NOT IN (SELECT CommunityId FROM Jobs)
                                            AND CommunityNumber NOT IN (SELECT CommunityNumber FROM imports.CommunityList);";

            using (var sc = new SqlConnection(connectionString))
            {
                sc.Open();

                using (var cmd = new SqlCommand("TRUNCATE TABLE imports.CommunityList", sc))
                    cmd.ExecuteNonQuery();
            }

            Import();

            using (var sc = new SqlConnection(connectionString))
            {
                sc.Open();

                using (var cmd = new SqlCommand(mergeScript, sc))
                {
                    cmd.CommandTimeout = 6000;
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}