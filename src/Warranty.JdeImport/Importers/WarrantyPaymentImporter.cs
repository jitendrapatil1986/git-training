using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace Warranty.JdeImport.Importers
{
    internal class WarrantyPaymentImporter : ImporterTemplate
    {
        public override string ItemName
        {
            get { return "Warranty Payments"; }
        }

        public override string SelectionQuery
        {
            get
            {
                return @"SELECT 
                                GLPN as PostingMonth
                                , GLCTRY || GLFY as PostingYear
                                , trim(GLMCU) as CostCenter
                                , GLOBJ as ObjectAccount
                                , GLSUB as CostCode
                                , GMDL01 as CostCodeDescription
                                , GLSBL as JobNumber
                                , GLVINV as InvoiceNumber
                                , qgpl.jded2date(GLDGJ) as DatePosted
                                , DECIMAL ( GLAA * 0.01 , 15 , 2 ) as Amount
                                , GLAN8 as VendorNumber
                                , GLEXA as ExplanationName
                                , GLEXR as ExplanationRemark
                                , GLDCT||'/'||TRIM(CHAR(GLDOC))||'/'||GLKCO||'/'||
                                    CHAR(QGPL.JDED2DATE(GLDGJ))||'/'||TRIM(CHAR(GLJELN))||'/'||TRIM(GLLT)||'/'||TRIM(GLEXTL) as JdeIdentifier
                            FROM F0911 l
                            INNER JOIN F0901 c
                                ON l.GLAID = c.GMAID
                            WHERE GLLT='AA'
                                AND GLCO IN ('00001', '00016')
                                AND GLOBJ IN ('9425', '9430', '9435', '9440')";
            }
        }

        public override string DestinationTable
        {
            get { return "imports.WarrantyPaymentStage"; }
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
                    new KeyValuePair<string, string>("PostingMonth", "PostingMonth"),
                    new KeyValuePair<string, string>("PostingYear", "PostingYear"),
                    new KeyValuePair<string, string>("CostCenter", "CostCenter"),
                    new KeyValuePair<string, string>("ObjectAccount", "ObjectAccount"),
                    new KeyValuePair<string, string>("CostCode", "CostCode"),
                    new KeyValuePair<string, string>("CostCodeDescription", "CostCodeDescription"),
                    new KeyValuePair<string, string>("JobNumber", "JobNumber"),
                    new KeyValuePair<string, string>("InvoiceNumber", "InvoiceNumber"),
                    new KeyValuePair<string, string>("DatePosted", "DatePosted"),
                    new KeyValuePair<string, string>("Amount", "Amount"),
                    new KeyValuePair<string, string>("VendorNumber", "VendorNumber"),
                    new KeyValuePair<string, string>("ExplanationName", "ExplanationName"),
                    new KeyValuePair<string, string>("ExplanationRemark", "ExplanationRemark"),
                    new KeyValuePair<string, string>("JdeIdentifier", "JdeIdentifier"),
                };
            }
        }

        public void CustomImport()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["WarrantyDB"].ConnectionString;

            const string mergeScript = @"MERGE INTO WarrantyPayments AS TARGET
                                            USING imports.WarrantyPaymentStage AS LIST
                                            ON TARGET.JdeIdentifier = LIST.JdeIdentifier
                                            WHEN NOT MATCHED BY TARGET THEN INSERT (PostingMonth, PostingYear, CostCenter, ObjectAccount, CostCode, CostCodeDescription, JobNumber, InvoiceNumber, DatePosted, Amount, VendorNumber, ExplanationName, ExplanationRemark, JdeIdentifier)
                                                                                VALUES (PostingMonth, PostingYear, CostCenter, ObjectAccount, CostCode, CostCodeDescription, JobNumber, InvoiceNumber, DatePosted, Amount, VendorNumber, ExplanationName, ExplanationRemark, JdeIdentifier)
                                            WHEN NOT MATCHED BY SOURCE THEN DELETE
                                            WHEN MATCHED THEN UPDATE SET  TARGET.PostingMonth = LIST.PostingMonth
                                                                        , TARGET.PostingYear = LIST.PostingYear
                                                                        , TARGET.CostCenter = LIST.CostCenter
                                                                        , TARGET.ObjectAccount = LIST.ObjectAccount
                                                                        , TARGET.CostCode = LIST.CostCode
                                                                        , TARGET.CostCodeDescription = LIST.CostCodeDescription
                                                                        , TARGET.JobNumber = LIST.JobNumber
                                                                        , TARGET.InvoiceNumber = LIST.InvoiceNumber
                                                                        , TARGET.DatePosted = LIST.DatePosted
                                                                        , TARGET.Amount = LIST.Amount
                                                                        , TARGET.VendorNumber = LIST.VendorNumber
                                                                        , TARGET.ExplanationName = LIST.ExplanationName
                                                                        , TARGET.ExplanationRemark = LIST.ExplanationRemark;";

            Import();

            using (var sc = new SqlConnection(connectionString))
            {
                sc.Open();

                using (var cmd = new SqlCommand(mergeScript, sc))
                {
                    cmd.CommandTimeout = 6000;
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new SqlCommand("TRUNCATE TABLE imports.WarrantyPaymentStage", sc))
                    cmd.ExecuteNonQuery();
            }
        }
    }
}