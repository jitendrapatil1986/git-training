using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace Warranty.JdeImport.Importers
{
    internal class AllVendorsImporter : ImporterTemplate
    {
        public override string ItemName
        {
            get { return "All Vendors"; }
        }

        public override string SelectionQuery
        {
            get
            {
                return @"SELECT rpmcu AS JOBNUMBER 
                            , COALESCE(TRIM(alph), '') AS VENDORNAME 
                            , COALESCE(e.EMAIL, '') AS VENDOREMAIL
                            , TRIM(COALESCE(rpan8 , '')) AS VENDORNUMBER 
                            , TRIM(COALESCE(wpphtp, '')) as PHONETYPE
                            , COALESCE(trim(wpar1) || trim(wpph1), '') as PHONENUMBER 
                            , gl01 AS CostCodeDESCRIPTION 
                            , rpsub AS COSTCODE 
                        FROM
                        (
                            SELECT f0.rpmcu,f0.rpsub ,MIN(gmdl01) AS gl01 ,f0.rpan8 ,MIN(abalph) AS alph ,CAST(SUM(rpag*.01) AS DECIMAL (15,2)) AS ag 
                            FROM   f0411 f0 
                            LEFT OUTER JOIN f0101 f1 ON f0.rpan8=f1.aban8
                            LEFT OUTER JOIN f0901 f2 ON f0.rpmcu=f2.gmmcu AND (CASE WHEN f0.rpobj=' ' THEN '1610' ELSE f0.rpobj END)=f2.gmobj AND f0.rpsub=f2.gmsub 
                            WHERE LENGTH(TRIM(f0.rpmcu)) = 8
                            GROUP BY f0.rpmcu,f0.rpsub,f0.rpan8 
                        ) a
                        LEFT OUTER JOIN F0115 F3  /* Phone Number */ 
                            ON rpan8=f3.wpan8 
                            AND  wpphtp in ('WORK','CAR','CELL','HOME',' ')
                        LEFT OUTER JOIN /* Email */
                            (SELECT email1.WPAN8, TRIM(email1.WPPH1) || COALESCE(TRIM(email2.WPPH1),'') || COALESCE(TRIM(email3.WPPH1), '') AS Email 
                                FROM F0115 email1
                                LEFT OUTER JOIN F0115 email2 ON
                                    email1.wpan8=email2.wpan8 AND email2.WPPHTP='EML2'
                                LEFT OUTER JOIN F0115 email3 ON
                                    email1.wpan8=email3.wpan8 AND email3.WPPHTP='EML3'
                                WHERE email1.WPPHTP='EML') e ON
                            e.WPAN8 = rpan8";
            }
        }

        public override string DestinationTable
        {
            get { return "imports.ArchivedVendorsStage"; }
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
                    new KeyValuePair<string, string>("JobNumber", "JobNumber"),
                    new KeyValuePair<string, string>("VendorName", "VendorName"),
                    new KeyValuePair<string, string>("VendorEmail", "VendorEmail"),
                    new KeyValuePair<string, string>("VendorNumber", "VendorNumber"),
                    new KeyValuePair<string, string>("PhoneType", "PhoneType"),
                    new KeyValuePair<string, string>("PhoneNumber", "PhoneNumber"),
                    new KeyValuePair<string, string>("CostCodeDescription", "CostCodeDescription"),
                    new KeyValuePair<string, string>("CostCode", "CostCode"),
                };
            }
        }

        public void CustomImport()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["WarrantyDB"].ConnectionString;

            const string mergeScript = @"MERGE INTO Vendors AS TARGET
                                            USING (SELECT DISTINCT LTRIM(RTRIM(VendorNumber)) AS VendorNumber, LTRIM(RTRIM(VendorName)) as VendorName FROM imports.ArchivedVendorsStage) AS LIST
                                            ON TARGET.Number = LIST.VendorNumber
                                            WHEN NOT MATCHED THEN 
                                                INSERT (Number, Name, CreatedBy, CreatedDate)
                                                VALUES (LIST.VendorNumber, LIST.VendorName, 'Warranty Jde Importer', GETDATE());

                                        MERGE INTO VendorEmails AS TARGET
                                            USING (SELECT DISTINCT V.VendorId, LTRIM(RTRIM(S.VendorEmail)) AS VendorEmail FROM
                                                    imports.ArchivedVendorsStage S
                                                    INNER JOIN Vendors V ON
                                                        LTRIM(RTRIM(S.VendorNumber)) = V.Number
                                                    WHERE LTRIM(RTRIM(VendorEmail)) != '') AS LIST
                                            ON TARGET.VendorId = LIST.VendorId AND TARGET.Email = LIST.VendorEmail
                                            WHEN NOT MATCHED THEN 
                                                INSERT (Email, VendorId, CreatedBy, CreatedDate)
                                                VALUES (LIST.VendorEmail, LIST.VendorId, 'Warranty Jde Importer', GETDATE());

                                        MERGE INTO VendorPhones AS TARGET
                                            USING (SELECT distinct v.vendorid, LTRIM(RTRIM(s.phonenumber)) AS PhoneNumber, LTRIM(RTRIM(s.phonetype)) AS PhoneType FROM
                                                    imports.ArchivedVendorsStage S
                                                    INNER JOIN Vendors V ON
                                                        LTRIM(RTRIM(S.VendorNumber)) = V.Number
                                                    WHERE LTRIM(RTRIM(PhoneNumber)) != '') AS LIST
                                            ON TARGET.VendorId = LIST.VendorId AND TARGET.Number = LTRIM(RTRIM(LIST.PhoneNumber))
                                            WHEN NOT MATCHED THEN 
                                                INSERT (Number, [Type], VendorId, CreatedBy, CreatedDate)
                                                VALUES (LIST.PhoneNumber, LIST.PhoneType, LIST.VendorId, 'Warranty Jde Importer', GETDATE());

                                        MERGE INTO JobVendorCostCodes AS TARGET
                                            USING (SELECT DISTINCT LTRIM(RTRIM(S.costcode)) as costcode, max(ltrim(rtrim(s.costcodedescription))) as costcodedescription, J.JobId, V.VendorId FROM
                                                    imports.ArchivedVendorsStage S
                                                    INNER JOIN Vendors V ON
                                                        LTRIM(RTRIM(S.VendorNumber)) = V.Number
                                                    INNER JOIN Jobs J ON
                                                        LTRIM(RTRIM(S.JobNumber)) = J.JobNumber
                                                    WHERE LTRIM(RTRIM(CostCode)) != ''
                                                    group by LTRIM(RTRIM(S.costcode)), j.jobid, v.vendorid) AS LIST
                                            ON TARGET.VendorId = LIST.VendorId AND TARGET.JobId = LIST.JobId AND TARGET.CostCode = List.CostCode
                                            WHEN NOT MATCHED THEN 
                                                INSERT (CostCode, CostCodeDescription, VendorId, JobId, CreatedBy, CreatedDate)
                                                VALUES (LIST.CostCode, LIST.CostCodeDescription, LIST.VendorId, LIST.JobId, 'Warranty Jde Importer', GETDATE());";

            using (var sc = new SqlConnection(connectionString))
            {
                sc.Open();

                using (var cmd = new SqlCommand("TRUNCATE TABLE imports.ArchivedVendorsStage", sc))
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