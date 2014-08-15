using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace Warranty.JdeImport.Importers
{
    internal class PaymentImporter : ImporterTemplate
    {
        public override string ItemName
        {
            get { return "Payments"; }
        }

        public override string SelectionQuery
        {
            get
            {
                return @"select 
                              T5AN8  as VendorNumber
                            , T5PAAP * .01 as Amount
                            , CASE WHEN T5TRDJ IS NULL THEN NULL ELSE qgpl.jded2date(T5TRDJ) END AS RequestedDate
                            , CASE WHEN T5FLAG=' ' THEN 'P' ELSE T5FLAG END as PaymentStatus --P = Pending Payment in Construction Portal
                            , trim(T5MCU) as JobNumber
                            , trim(T5MCU) || '/' || trim(T5$OPT) || '/' || trim(T5R006) || '/' || trim(T5SUB) || '/' || trim(T5OBJ) || '/' || trim(T5DCTO) || '/' || digits(T5DOCO) || '/' || digits(T5LNID) || '/' || trim(T5SFX) as JdeIdentifier
                            , '" + DateTime.UtcNow + @"' as CreatedDate
                            , 'Warranty Jde Import' as CreatedBy
                        from f58235 p";
            }
        }

        public override string DestinationTable
        {
            get { return "imports.PaymentStage"; }
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
                    new KeyValuePair<string, string>("VendorNumber", "VendorNumber"),
                    new KeyValuePair<string, string>("Amount", "Amount"),
                    new KeyValuePair<string, string>("RequestedDate","RequestedDate"),
                    new KeyValuePair<string, string>("PaymentStatus", "PaymentStatus"),
                    new KeyValuePair<string, string>("JobNumber", "JobNumber"),
                    new KeyValuePair<string, string>("JdeIdentifier", "JdeIdentifier"),
                    new KeyValuePair<string, string>("CreatedDate", "CreatedDate"),
                    new KeyValuePair<string, string>("CreatedBy", "CreatedBy"),
                };
            }
        }

        public void CustomImport()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["WarrantyDB"].ConnectionString;

            const string addIndex = @"IF NOT EXISTS (SELECT 1 FROM SYS.INDEXES WHERE NAME = 'IDX_PaymentStage_JdeId') CREATE NONCLUSTERED INDEX IDX_PaymentStage_JdeId ON imports.PaymentStage(JdeIdentifier)";
            const string dropIndex = @"DROP INDEX IDX_PaymentStage_JdeId ON imports.PaymentStage";

            const string mergeScript = @";WITH stage AS (SELECT * FROM (
                                                                  SELECT *
                                                                          , ROW_NUMBER() OVER (PARTITION BY JdeIdentifier ORDER BY RequestedDate) AS rowNum
                                                                  FROM imports.PaymentStage
                                                              ) removeDups
                                                              WHERE rowNum = 1)
                                          MERGE INTO Payments AS TARGET
                                          USING stage AS LIST
                                          ON TARGET.JdeIdentifier = LIST.JdeIdentifier
                                          WHEN NOT MATCHED BY TARGET THEN INSERT (VendorNumber, Amount, PaymentStatus, JobNumber, JdeIdentifier, CreatedDate, CreatedBy, RequestedDate)
                                                                              VALUES (VendorNumber, Amount, PaymentStatus, JobNumber, JdeIdentifier, CreatedDate, CreatedBy, RequestedDate)
                                          WHEN NOT MATCHED BY SOURCE THEN DELETE
                                          WHEN MATCHED THEN UPDATE SET TARGET.VendorNumber = LIST.VendorNumber
                                                                      , TARGET.Amount = LIST.Amount
                                                                      , TARGET.PaymentStatus = LIST.PaymentStatus
                                                                      , TARGET.JobNumber = LIST.JobNumber
                                                                      , TARGET.JdeIdentifier = LIST.JdeIdentifier
                                                                      , TARGET.CreatedDate = LIST.CreatedDate
                                                                      , TARGET.CreatedBy = LIST.CreatedBy;";

            Import();

            using (var sc = new SqlConnection(connectionString))
            {
                sc.Open();

                using (var cmd = new SqlCommand(addIndex, sc))
                {
                    cmd.CommandTimeout = 600;
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new SqlCommand(mergeScript, sc))
                {
                    cmd.CommandTimeout = 600;
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new SqlCommand("TRUNCATE TABLE imports.PaymentStage", sc))
                    cmd.ExecuteNonQuery();

                using (var cmd = new SqlCommand(dropIndex, sc))
                    cmd.ExecuteNonQuery();
            }
        }
    }
}