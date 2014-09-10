using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace Warranty.JdeImport.Importers
{
    internal class JobImporter : ImporterTemplate
    {
        public override string ItemName
        {
            get { return "Jobs"; }
        }

        public override string SelectionQuery
        {
            get
            {
                return @"SELECT 
                                   trim(f1.$hmcu) as JobNumber
                                 , min(case when $H$CDJ = 0 OR $H$CDJ IS NULL then null else qgpl.jded2date($H$CDJ) end) as CloseDate
                                 , min(case when f1.$h$pdj = 0 OR f1.$h$pdj IS NULL then null else qgpl.jded2date(f1.$h$pdj) end) as StartDate
                                 , min(case when f2.mcd3j = 0 OR f2.mcd3j IS NULL then null else qgpl.jded2date(f2.mcd3j) end) as CompletionDate
                                 , min(case when $H$SDJ = 0 OR $H$SDJ IS NULL then null else qgpl.jded2date($H$SDJ) end) as SalesDate
                                 , min(case when $H$F2J = 0 OR $H$F2J IS NULL then null else qgpl.jded2date($H$F2J) end) as StartPostDate
                                 , trim(min(jobadd.aladd1)) || ' ' || trim(min(jobadd.aladd2)) as AddressLine
                                 , trim(min(jobadd.alcty1)) as City
                                 , trim(min(jobadd.aladds)) as StateCode
                                 , trim(min(jobadd.aladdz)) as PostalCode
                                 , trim(min($2$COM)) as LegalDescription
                                 , trim(min($H$MCU)) as CommunityNumber
                                 , trim(min(f2c.MCDL01)) as CommunityName
                                 , trim(min(f2c.MCDL02)) as CommunityMarketingName
                                 , trim(min(f1.$h$byr)) as BuyerNumber
                                 , trim(min(f4.abalph)) as BuyerName
                                 , trim(min(f5.aladd1)) || ' ' || trim(min(f5.aladd2)) as BuyerAddressLine
                                 , trim(min(f5.alcty1)) as BuyerCity
                                 , trim(min(f5.aladds)) as BuyerStateCode
                                 , trim(min(f5.aladdz)) as BuyerPostalCode
                                 , trim(min(e.email)) as BuyerEmail
                                 , trim(min(des.$1$FE4)) as PlanType
                                 , case trim(min(des.$1$FE4)) when 'SNG' then 'Single Home' when 'UNT' then 'Unit' ELSE '' end as PlanTypeDescription
                                 , trim(max(case when cpl.$1$edc <> ' ' then cpl.$1$edc else des.$1$edc end)) as PlanName
                                 , trim(min(f1.$h$pln)) as PlanNumber
                                 , trim(min(f1.$h$elv)) as Elevation
                                 , trim(min(f1.$h$dvw)) as Swing
                                 , max(f2.MCPC*100) as Stage
                                 , trim(substring(min(f1.$h$bld),3,8)) as BuilderEmployeeNumber
                                 , trim(replace(min(substr(f6.abalph,max(posstr(f6.abalph,',')+1,1),40)) , '**TERMED',' ')) || ' ' || trim(min(substr(f6.abalph,1,max(posstr(f6.abalph,',')-1,1) ))) as BuilderEmployeeName
                                 , trim(substring(cast(max($H$ASC) AS VARCHAR(8)), 3, 8)) as SalesConsultantEmployeeNumber
                                 , trim(replace(min(substr(sales.abalph,max(posstr(sales.abalph,',')+1,1),40)) , '**TERMED',' ')) || ' ' || trim(min(substr(sales.abalph,1,max(posstr(sales.abalph,',')-1,1) ))) as SalesConsultantEmployeeName
                                 , trim(f1.$hmcu) as JdeIdentifier
                            FROM F58300 f1
                            INNER JOIN F0006 f2
                                  ON f1.$hmcu = f2.mcmcu
                            INNER JOIN F0006 f2c
                                  ON f1.$H$MCU = f2c.mcmcu
                            INNER JOIN F57002 f3
                                  ON f1.$hmcu = f3.$2mcu
                            LEFT OUTER JOIN F0101 f4
                                  ON f1.$h$byr = f4.aban8
                            LEFT OUTER JOIN F0116 f5
                                  ON f4.aban8 = f5.alan8 AND f4.abeftb = f5.aleftb
                            LEFT OUTER JOIN F0101 F6
                                  ON f1.$h$bld = f6.aban8
                            LEFT OUTER JOIN F0101 F7
                                  ON f1.$h$ds2 = f7.aban8
                            LEFT OUTER JOIN 
                                  (SELECT email1.WPAN8, trim(email1.WPPH1) || COALESCE(trim(email2.WPPH1),'') || COALESCE(trim(email3.WPPH1), '') as email 
                                      FROM F0115 email1
                                      LEFT OUTER JOIN F0115 email2
                                            ON email1.wpan8=email2.wpan8 AND email2.WPPHTP='EML2'
                                      LEFT OUTER JOIN F0115 email3
                                            ON email1.wpan8=email3.wpan8 AND email3.WPPHTP='EML3'
                                      WHERE email1.WPPHTP='EML') e
                                  ON e.WPAN8 = $h$byr
                            INNER JOIN F57001 cpl
                                  ON f1.$h$mcu = cpl.$1$mcu AND f1.$h$pln = cpl.$1$pev AND f1.$h$elv = cpl.$1$hse
                            LEFT OUTER JOIN F57001 des
                                  ON des.$1$mcu = '      DESIGN' AND $h$pln = des.$1$pev AND SUBSTRING($h$elv,1,1) = CASE WHEN SUBSTRING(des.$1$hse,1,1) = '' THEN SUBSTRING($h$elv,1,1) ELSE SUBSTRING(des.$1$hse,1,1) END
                            INNER JOIN f0101 job
                                  ON f1.$hmcu = '    ' || job.aban8
                            INNER JOIN F0116 jobadd
                                  ON job.aban8 = jobadd.alan8 AND job.abeftb = jobadd.aleftb
                            LEFT OUTER JOIN f0101 designer
                                  ON $H$DS1 = designer.aban8
                            LEFT OUTER JOIN f0101 sales
                                  ON $H$ASC = sales.aban8
                            WHERE f2.mcstyl='JB' 
                            GROUP BY $hmcu";
            }
        }

        public override string DestinationTable
        {
            get { return "imports.JobStage"; }
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
                    new KeyValuePair<string, string>("CloseDate","CloseDate"),
                    new KeyValuePair<string, string>("StartDate","StartDate"),
                    new KeyValuePair<string, string>("CompletionDate","CompletionDate"),
                    new KeyValuePair<string, string>("SalesDate","SalesDate"),
                    new KeyValuePair<string, string>("StartPostDate","StartPostDate"),
                    new KeyValuePair<string, string>("AddressLine","AddressLine"),
                    new KeyValuePair<string, string>("City","City"),
                    new KeyValuePair<string, string>("StateCode","StateCode"),
                    new KeyValuePair<string, string>("PostalCode","PostalCode"),
                    new KeyValuePair<string, string>("LegalDescription","LegalDescription"),
                    new KeyValuePair<string, string>("CommunityNumber","CommunityNumber"),
                    new KeyValuePair<string, string>("CommunityName","CommunityName"),
                    new KeyValuePair<string, string>("CommunityMarketingName","CommunityMarketingName"),
                    new KeyValuePair<string, string>("BuyerNumber","BuyerNumber"),
                    new KeyValuePair<string, string>("BuyerName","BuyerName"),
                    new KeyValuePair<string, string>("BuyerAddressLine","BuyerAddressLine"),
                    new KeyValuePair<string, string>("BuyerCity","BuyerCity"),
                    new KeyValuePair<string, string>("BuyerStateCode","BuyerStateCode"),
                    new KeyValuePair<string, string>("BuyerPostalCode","BuyerPostalCode"),
                    new KeyValuePair<string, string>("BuyerEmail","BuyerEmail"),
                    new KeyValuePair<string, string>("PlanType","PlanType"),
                    new KeyValuePair<string, string>("PlanTypeDescription","PlanTypeDescription"),
                    new KeyValuePair<string, string>("PlanName","PlanName"),
                    new KeyValuePair<string, string>("PlanNumber","PlanNumber"),
                    new KeyValuePair<string, string>("Elevation","Elevation"),
                    new KeyValuePair<string, string>("Swing","Swing"),
                    new KeyValuePair<string, string>("Stage","Stage"),
                    new KeyValuePair<string, string>("BuilderEmployeeNumber","BuilderEmployeeNumber"),
                    new KeyValuePair<string, string>("BuilderEmployeeName","BuilderEmployeeName"),
                    new KeyValuePair<string, string>("SalesConsultantEmployeeNumber","SalesConsultantEmployeeNumber"),
                    new KeyValuePair<string, string>("SalesConsultantEmployeeName","SalesConsultantEmployeeName"),
                    new KeyValuePair<string, string>("JdeIdentifier","JdeIdentifier"),
                };
            }
        }

        public void CustomImport()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["WarrantyDB"].ConnectionString;

            const string addIndex = @"IF NOT EXISTS (SELECT 1 FROM SYS.INDEXES WHERE NAME = 'IDX_JobStage_JdeId') CREATE NONCLUSTERED INDEX IDX_JobStage_JdeId ON imports.JobStage(JdeIdentifier)";
            const string dropIndex = @"DROP INDEX IDX_JobStage_JdeId ON imports.JobStage";

            const string mergeScript = @"DECLARE @ImportUser VARCHAR(255) = 'Warranty Jde Import';

                                            MERGE INTO Employees AS TARGET
                                            USING (
                                                    SELECT Number, MIN(Name) AS Name FROM (
                                                        SELECT DISTINCT BuilderEmployeeNumber AS Number, BuilderEmployeeName AS Name
                                                        FROM imports.JobStage
       
                                                        UNION 
       
                                                        SELECT DISTINCT SalesConsultantEmployeeNumber, SalesConsultantEmployeeName
                                                        FROM imports.JobStage
                                                    ) EmpList GROUP BY Number) AS LIST
                                            ON TARGET.EmployeeNumber = LIST.Number
                                            WHEN NOT MATCHED BY TARGET THEN INSERT (EmployeeNumber, EmployeeName)
                                                                                VALUES (Number, Name)
                                            WHEN MATCHED THEN UPDATE SET EmployeeName = Name;

                                            ;WITH nonDupEmps AS (SELECT MIN(EmployeeId) EmployeeId, EmployeeNumber FROM employees GROUP BY EmployeeNumber),
                                                  stage AS (SELECT removeDups.*
                                                                    , builder.EmployeeId as BuilderId
                                                                    , sales.EmployeeId as SalesId
                                                                    , COM.CommunityId
                                                            FROM (
                                                                            SELECT *
                                                                                    , ROW_NUMBER() OVER (PARTITION BY JdeIdentifier ORDER BY JobStageId DESC) AS rowNum
                                                                            FROM imports.JobStage
                                                                        ) removeDups
                                                                    INNER JOIN employees builder ON
                                                                        removeDups.BuilderEmployeeNumber = builder.EmployeeNumber
                                                                    INNER JOIN employees sales ON
                                                                        removeDups.SalesConsultantEmployeeNumber = sales.EmployeeNumber
                                                                    INNER JOIN Communities com ON
                                                                        LEFT(removeDups.CommunityNumber, 4) = com.CommunityNumber
                                                            WHERE rowNum = 1)
                                            MERGE INTO Jobs AS TARGET
                                            USING stage AS LIST
                                            ON TARGET.JdeIdentifier = LIST.JdeIdentifier
                                            WHEN NOT MATCHED BY TARGET THEN INSERT (JobNumber, CloseDate, AddressLine, City, StateCode, PostalCode, LegalDescription, CommunityId, PlanType, PlanTypeDescription, PlanName, PlanNumber, Elevation, Swing, Stage, BuilderEmployeeId, SalesConsultantEmployeeId, WarrantyExpirationDate, CreatedDate, CreatedBy, JdeIdentifier)
                                                                                VALUES (JobNumber, CloseDate, AddressLine, City, StateCode, PostalCode, LegalDescription, CommunityId, PlanType, PlanTypeDescription, PlanName, PlanNumber, Elevation, Swing, Stage, BuilderId, SalesId, CloseDate, SYSDATETIME(), @ImportUser, JdeIdentifier)
                                            WHEN MATCHED THEN UPDATE SET    TARGET.CloseDate = LIST.CloseDate
                                                                            , TARGET.AddressLine = LIST.AddressLine
                                                                            , TARGET.City = LIST.City
                                                                            , TARGET.StateCode = LIST.StateCode
                                                                            , TARGET.PostalCode = LIST.PostalCode
                                                                            , TARGET.LegalDescription = LIST.LegalDescription
                                                                            , TARGET.CommunityId = LIST.CommunityId                                
                                                                            , TARGET.PlanType = LIST.PlanType
                                                                            , TARGET.PlanTypeDescription = LIST.PlanTypeDescription
                                                                            , TARGET.PlanName = LIST.PlanName
                                                                            , TARGET.PlanNumber = LIST.PlanNumber
                                                                            , TARGET.Elevation = LIST.Elevation
                                                                            , TARGET.Swing = LIST.Swing
                                                                            , TARGET.Stage = LIST.Stage
                                                                            , TARGET.BuilderEmployeeId = LIST.BuilderId
                                                                            , TARGET.SalesConsultantEmployeeId = LIST.SalesId
                                                                            , TARGET.WarrantyExpirationDate = DATEADD(YY, 10, LIST.CloseDate)
                                                                            , TARGET.UpdatedDate = SYSDATETIME()
                                                                            , TARGET.UpdatedBy = @ImportUser;

                                            MERGE INTO HomeOwners AS TARGET
                                            USING (SELECT DISTINCT 
                                                        J.JobId
                                                        , 1 as HomeOwnerNumber
                                                        , JS.BuyerName
                                                        , JS.BuyerEmail
                                                    FROM imports.JobStage JS
                                                    INNER JOIN Jobs J ON
                                                        JS.JobNumber = J.JobNumber
                                                    ) AS LIST
                                            ON TARGET.JobId = LIST.JobId
                                            WHEN NOT MATCHED THEN INSERT (JobId, HomeOwnerNumber, HomeOwnerName, EmailAddress, CreatedDate, CreatedBy)
                                                                    VALUES (LIST.JobId, HomeOwnerNumber, BuyerName, BuyerEmail, SYSDATETIME(), @ImportUser);

                                            UPDATE Jobs SET CurrentHomeOwnerId = HomeOwnerId
                                            FROM HomeOwners
                                            WHERE Jobs.JobId = HomeOwners.JobId
                                            AND Jobs.CurrentHomeOwnerId IS NULL;";

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
                    cmd.CommandTimeout = 600;
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new SqlCommand("TRUNCATE TABLE imports.JobStage", sc))
                   cmd.ExecuteNonQuery();

                using (var cmd = new SqlCommand(dropIndex, sc))
                    cmd.ExecuteNonQuery();
            }
        }
    }
}