using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace Warranty.JdeImport.Importers
{
    internal class JobStageImporter : ImporterTemplate
    {
        public override string ItemName
        {
            get { return "Job Stages"; }
        }

        public override string SelectionQuery
        {
            get
            {
                return @"SELECT DISTINCT
                         trim(f3.GHMCU) as JobNumber
                         , f3.GH$STG as JobStage
                         , max(case when f3.GHATFN = 0 OR f3.GHATFN IS NULL then null else qgpl.jded2date(GHATFN) end) AS CompletionDate
                         , TRIM (f3.GHMCU ) || '/' || DIGITS (f3.GH$STG ) as JdeIdentifier
                         FROM F58300 f1
                         INNER JOIN F0006 f2
                                 ON f1.$hmcu = f2.mcmcu
                         INNER JOIN F5547H f3
                                 ON f1.$hmcu = f3.GHMCU
                         WHERE f2.mcstyl IN ('JB', 'BD')
                         GROUP BY f3.GHMCU, f3.GH$STG";
            }
        }

        public override string DestinationTable
        {
            get { return "imports.JobStageImports"; }
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
                    new KeyValuePair<string, string>("JobNumber","JobNumber"),
                    new KeyValuePair<string, string>("JobStage","JobStage"),
                    new KeyValuePair<string, string>("CompletionDate","CompletionDate"),
                    new KeyValuePair<string, string>("JdeIdentifier","JdeIdentifier"),
                };
            }
        }

        public void CustomImport()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["WarrantyDB"].ConnectionString;

            const string mergeScript = @"MERGE INTO JobStages AS TARGET
                                            USING (SELECT i.*, j.JobId
                                                    FROM imports.JobStageImports i
                                                    INNER JOIN Jobs j ON
                                                        i.JobNumber = j.JobNumber) AS LIST
                                            ON TARGET.JdeIdentifier = LIST.JdeIdentifier
                                            WHEN MATCHED THEN 
                                                UPDATE SET TARGET.CompletionDate = LIST.CompletionDate,
                                                            TARGET.UpdatedDate = SYSDATETIME(),
                                                            TARGET.UpdatedBy = 'Warranty Jde Import'
                                            WHEN NOT MATCHED THEN
                                                INSERT (JobId, Stage, CompletionDate, JdeIdentifier, CreatedDate, CreatedBy)
                                                VALUES (JobId, JobStage, CompletionDate, JdeIdentifier, SYSDATETIME(), 'Warranty Jde Import');";

            using (var sc = new SqlConnection(connectionString))
            {
                sc.Open();

                using (var cmd = new SqlCommand("TRUNCATE TABLE imports.JobStageImports", sc))
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
