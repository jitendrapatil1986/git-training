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
                            T5AN8 as VendorNumber
                            , T5PAAP * .01 as Amount
                            , CASE WHEN T5TRDJ IS NULL THEN NULL ELSE qgpl.jded2date(T5TRDJ) END AS CreatedDate
                            , 'Warranty JDE Import' AS CreatedBy
                            , CASE WHEN T5FLAG=' ' THEN 'P' ELSE T5FLAG END as PaymentStatus --P = Pending Payment in Construction Portal
                            , trim(T5MCU) as CommunityNumber
                            , trim(T5MCU) || '/' || trim(T5$OPT) || '/' || trim(T5R006) || '/' || trim(T5SUB) || '/' || trim(T5OBJ) || '/' || trim(T5DCTO) || '/' || digits(T5DOCO) || '/' || digits(T5LNID) || '/' || trim(T5SFX) as JdeIdentifier
                            , CASE WHEN T5SBLT='A' THEN T5$OPT ELSE null END as JobNumber
                            , T5VINV as InvoiceNumber
                            , trim(coalesce(m1.T6SRDS, '')) || trim(coalesce(m2.T6SRDS, '')) || trim(coalesce(m3.T6SRDS, '')) || trim(coalesce(m4.T6SRDS, '')) as HoldComments
                            , trim(coalesce(vp1.T6SRDS, '')) || trim(coalesce(vp2.T6SRDS, '')) || trim(coalesce(vp3.T6SRDS, '')) || trim(coalesce(vp4.T6SRDS, '')) as VarianceExplanation
                            from f58235 p
                            left outer join f58235M m1 on p.T5MCU = m1.T6MCU 
                                and p.T5$OPT = m1.T6$OPT 
                                and p.T5SUB = m1.T6SUB 
                                and p.T5OBJ = m1.T6OBJ 
                                and p.T5DCTO = m1.T6DCTO 
                                and p.T5DOCO = m1.T6DOCO 
                                and p.T5SFX = m1.T6SFX 
                                and trim(m1.T6$LIN) = 1 
                                and trim(m1.T6FLAG) = 'H'
                            left outer join f58235M m2 on p.T5MCU = m2.T6MCU 
                                and p.T5$OPT = m2.T6$OPT 
                                and p.T5SUB = m2.T6SUB 
                                and p.T5OBJ = m2.T6OBJ 
                                and p.T5DCTO = m2.T6DCTO 
                                and p.T5DOCO = m2.T6DOCO 
                                and p.T5SFX = m2.T6SFX 
                                and trim(m2.T6$LIN) = 2 
                                and trim(m2.T6FLAG) = 'H'
                            left outer join f58235M m3 on p.T5MCU = m3.T6MCU 
                                and p.T5$OPT = m3.T6$OPT 
                                and p.T5SUB = m3.T6SUB 
                                and p.T5OBJ = m3.T6OBJ 
                                and p.T5DCTO = m3.T6DCTO 
                                and p.T5DOCO = m3.T6DOCO 
                                and p.T5SFX = m3.T6SFX 
                                and trim(m3.T6$LIN) = 3 
                                and trim(m3.T6FLAG) = 'H'
                            left outer join f58235M m4 on p.T5MCU = m4.T6MCU 
                                and p.T5$OPT = m4.T6$OPT 
                                and p.T5SUB = m4.T6SUB 
                                and p.T5OBJ = m4.T6OBJ 
                                and p.T5DCTO = m4.T6DCTO 
                                and p.T5DOCO = m4.T6DOCO 
                                and p.T5SFX = m4.T6SFX 
                                and trim(m4.T6$LIN) = 4 
                                and trim(m4.T6FLAG) = 'H'
                            left outer join f58235M vp1 on p.T5MCU = vp1.T6MCU 
                                and p.T5$OPT = vp1.T6$OPT 
                                and p.T5SUB = vp1.T6SUB 
                                and p.T5OBJ = vp1.T6OBJ 
                                and p.T5DCTO = vp1.T6DCTO 
                                and p.T5DOCO = vp1.T6DOCO 
                                and p.T5SFX = vp1.T6SFX 
                                and trim(vp1.T6$LIN) = 1
                                and trim(vp1.T6PDS1) <> ''
                                and trim(vp1.T6DCTO)  ='$W'
                            left outer join f58235M vp2 on p.T5MCU = vp2.T6MCU 
                                and p.T5$OPT = vp2.T6$OPT 
                                and p.T5SUB = vp2.T6SUB 
                                and p.T5OBJ = vp2.T6OBJ 
                                and p.T5DCTO = vp2.T6DCTO 
                                and p.T5DOCO = vp2.T6DOCO 
                                and p.T5SFX = vp2.T6SFX 
                                and trim(vp2.T6$LIN) = 2
                                and trim(vp2.T6PDS1) <> ''
                                and trim(vp2.T6DCTO) = '$W'
                            left outer join f58235M vp3 on p.T5MCU = vp3.T6MCU 
                                and p.T5$OPT = vp3.T6$OPT 
                                and p.T5SUB = vp3.T6SUB 
                                and p.T5OBJ = vp3.T6OBJ 
                                and p.T5DCTO = vp3.T6DCTO 
                                and p.T5DOCO = vp3.T6DOCO 
                                and p.T5SFX = vp3.T6SFX 
                                and trim(vp3.T6$LIN) = 3
                                and trim(vp3.T6PDS1) <> ''
                                and trim(vp3.T6DCTO) = '$W'
                            left outer join f58235M vp4 on p.T5MCU = vp4.T6MCU 
                                and p.T5$OPT = vp4.T6$OPT 
                                and p.T5SUB = vp4.T6SUB 
                                and p.T5OBJ = vp4.T6OBJ 
                                and p.T5DCTO = vp4.T6DCTO 
                                and p.T5DOCO = vp4.T6DOCO 
                                and p.T5SFX = vp4.T6SFX 
                                and trim(vp4.T6$LIN) = 4
                                and trim(vp4.T6PDS1) <> ''
                                and trim(vp4.T6DCTO) = '$W'
                                WHERE TRIM(T5OBJ) IN ('9425', '9430', '9435', '9440')";
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
                    new KeyValuePair<string, string>("PaymentStatus", "PaymentStatus"),
                    new KeyValuePair<string, string>("CommunityNumber", "CommunityNumber"),
                    new KeyValuePair<string, string>("JobNumber", "JobNumber"),
                    new KeyValuePair<string, string>("InvoiceNumber", "InvoiceNumber"),
                    new KeyValuePair<string, string>("JdeIdentifier", "JdeIdentifier"),
                    new KeyValuePair<string, string>("HoldComments", "HoldComments"),
                    new KeyValuePair<string, string>("VarianceExplanation", "VarianceExplanation"),
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
                                                                          , ROW_NUMBER() OVER (PARTITION BY JdeIdentifier ORDER BY CreatedDate) AS rowNum
                                                                  FROM imports.PaymentStage
                                                              ) removeDups
                                                              WHERE rowNum = 1)
                                          MERGE INTO Payments AS TARGET
                                          USING stage AS LIST
                                          ON TARGET.JdeIdentifier = LIST.JdeIdentifier
                                          WHEN NOT MATCHED BY TARGET THEN INSERT (VendorNumber, Amount, PaymentStatus, JobNumber, JdeIdentifier, CreatedDate, CreatedBy, CommunityNumber, InvoiceNumber, HoldComments, VarianceExplanation)
                                                                              VALUES (VendorNumber, Amount, PaymentStatus, JobNumber, JdeIdentifier, CreatedDate, CreatedBy, CommunityNumber, InvoiceNumber, HoldComments, VarianceExplanation)
                                          WHEN NOT MATCHED BY SOURCE THEN DELETE
                                          WHEN MATCHED THEN UPDATE SET TARGET.VendorNumber = LIST.VendorNumber
                                                                      , TARGET.Amount = LIST.Amount
                                                                      , TARGET.PaymentStatus = LIST.PaymentStatus
                                                                      , TARGET.JobNumber = LIST.JobNumber
                                                                      , TARGET.JdeIdentifier = LIST.JdeIdentifier
                                                                      , TARGET.CreatedDate = LIST.CreatedDate
                                                                      , TARGET.CreatedBy = LIST.CreatedBy
                                                                      , TARGET.CommunityNumber = LIST.CommunityNumber
                                                                      , TARGET.InvoiceNumber = LIST.InvoiceNumber
                                                                      , TARGET.HoldComments = LIST.HoldComments
                                                                      , TARGET.VarianceExplanation = LIST.VarianceExplanation;";

            Import();

            using (var sc = new SqlConnection(connectionString))
            {
                sc.Open();

                using (var cmd = new SqlCommand(addIndex, sc))
                {
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new SqlCommand(mergeScript, sc))
                {
                    cmd.CommandTimeout = 6000;
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