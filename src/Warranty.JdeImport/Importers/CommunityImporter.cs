using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace Warranty.JdeImport.Importers
{
    internal class CommunityImporter : ImporterTemplate
    {
        public override string ItemName
        {
            get { return "Communities"; }
        }

        public override string SelectionQuery
        {
            get
            {
                return @"SELECT WFCITY, WFDIVN, WFPMNM, WFRP23, WFMCUN, WFNM08, WFRP08, WFRP06, Wfrp07, Wfnm07, WFPRTY, WFNM06, WFRP09, WFDIVE, WFPMEE, WFNM09, WFRP01, WFRP02, WFRP14
                            FROM QGPL.F55285DL";
            }
        }

        public override string DestinationTable
        {
            get { return "imports.CommunityStage"; }
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
                    new KeyValuePair<string, string>("WFCITY","City"),
                    new KeyValuePair<string, string>("WFDIVN","Division"),
                    new KeyValuePair<string, string>("WFPMNM","Project"),
                    new KeyValuePair<string, string>("WFRP23","Comm_Num"),
                    new KeyValuePair<string, string>("WFMCUN","Community"),
                    new KeyValuePair<string, string>("WFNM08","Satelite"),
                    new KeyValuePair<string, string>("WFRP08","SateliteCode"),
                    new KeyValuePair<string, string>("WFRP06","AreaPresCode"),
                    new KeyValuePair<string, string>("Wfrp07","StatusCode"),
                    new KeyValuePair<string, string>("Wfnm07","StatusDescription"),
                    new KeyValuePair<string, string>("WFPRTY","TypeCC"),
                    new KeyValuePair<string, string>("WFNM06","AreaPres"),
                    new KeyValuePair<string, string>("WFRP09","BOYL"),
                    new KeyValuePair<string, string>("WFDIVE","DivPresEmpID"),
                    new KeyValuePair<string, string>("WFPMEE","PMEmpID"),
                    new KeyValuePair<string, string>("WFNM09","PlantypeDesc"),
                    new KeyValuePair<string, string>("WFRP01","DivCode"),
                    new KeyValuePair<string, string>("WFRP02","ProjectCode"),
                    new KeyValuePair<string, string>("WFRP14","CityCode"),
                };
            }
        }

        public void CustomImport()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["WarrantyDB"].ConnectionString;

            const string mergeScript = @"DECLARE @ImportUser VARCHAR(255) = 'Warranty Jde Import';

                                        MERGE INTO Divisions AS TARGET
                                        USING (SELECT
                                                        importId AS rowId,
                                                        DivisionCode,
                                                        Division,
                                                        AreaName,
                                                        AreaCode
                                                    FROM (SELECT
                                                                ROW_NUMBER() OVER (PARTITION BY RTRIM(LTRIM(DivCode)) ORDER BY MIN(RowId) DESC) AS rowNumber,
                                                                MIN(RowId) AS ImportId,
                                                                RTRIM(LTRIM(DivCode)) AS DivisionCode,
                                                                RTRIM(LTRIM(Division)) AS Division,
                                                                RTRIM(LTRIM(AreaPres)) AS AreaName,
                                                                RTRIM(LTRIM(AreaPresCode)) AS AreaCode
                                                            FROM imports.CommunityStage
                                                            WHERE RTRIM(LTRIM(DivCode)) != ''
                                                            GROUP BY RTRIM(LTRIM(DivCode)), RTRIM(LTRIM(Division)), RTRIM(LTRIM(AreaPres)), RTRIM(LTRIM(AreaPresCode))) AS Div
                                                    WHERE rowNumber = 1
                                                            ) AS LIST
                                        ON TARGET.DivisionCode = LIST.DivisionCode
                                        WHEN NOT MATCHED BY TARGET THEN INSERT (DivisionId
                                                                        , DivisionCode
                                                                        , DivisionName
                                                                        , AreaCode
                                                                        , AreaName
                                                                        , CreatedDate
                                                                        , CreatedBy)
                                                                VALUES (rowId
                                                                        , DivisionCode
                                                                        , Division
                                                                        , AreaCode
                                                                        , AreaName
                                                                        , GETDATE()
                                                                        , @ImportUser)
                                        WHEN MATCHED THEN UPDATE SET AreaCode = LIST.AreaCode
                                                                    , AreaName = LIST.AreaName
                                                                    , UpdatedDate = GETDATE()
                                                                    , UpdatedBy = @ImportUser;

                                        MERGE INTO Cities AS TARGET
                                        USING (SELECT
                                                    ImportId AS rowId,
                                                    cityCode,
                                                    city
                                                FROM (SELECT 
                                                            MIN(RowId) AS ImportId,
                                                            LTRIM(RTRIM(CityCode)) AS CityCode,
                                                            LTRIM(RTRIM(City)) AS City
                                                        FROM imports.CommunityStage
                                                        WHERE LEN(LTRIM(RTRIM(CityCode))) = 3
                                                        GROUP BY LTRIM(RTRIM(CityCode)), LTRIM(RTRIM(City))) AS City) AS LIST
                                        ON TARGET.CityCode = LIST.CityCode
                                            AND TARGET.CityName = LIST.City
                                        WHEN NOT MATCHED BY TARGET THEN INSERT (CityId
                                                                        , CityCode
                                                                        , CityName
                                                                        , CreatedDate
                                                                        , CreatedBy)
                                                                VALUES (rowId
                                                                        , CityCode
                                                                        , City
                                                                        , GETDATE()
                                                                        , @ImportUser);

                                        MERGE INTO Projects AS TARGET
                                        USING (SELECT
                                                    ImportId AS rowId,
                                                    projectCode,
                                                    project
                                                FROM (SELECT 
                                                        ROW_NUMBER() OVER (PARTITION BY LTRIM(RTRIM(ProjectCode)) ORDER BY MIN(RowId) DESC) AS rowNumber,
                                                        MIN(RowId) AS ImportId,
                                                        LTRIM(RTRIM(ProjectCode)) AS ProjectCode,
                                                        LTRIM(RTRIM(Project)) Project 
                                                        FROM imports.CommunityStage
                                                        WHERE LTRIM(RTRIM(ProjectCode)) != ''
                                                        GROUP BY LTRIM(RTRIM(ProjectCode)), LTRIM(RTRIM(Project))) AS Proj
                                                WHERE rowNumber = 1) AS LIST
                                        ON TARGET.ProjectNumber = LIST.ProjectCode
                                        WHEN MATCHED THEN UPDATE SET ProjectName = Project, UpdatedDate = GETUTCDATE(), UpdatedBy = @ImportUser
                                        WHEN NOT MATCHED THEN INSERT (ProjectId
                                                                        , ProjectNumber
                                                                        , ProjectName
                                                                        , CreatedDate
                                                                        , CreatedBy)
                                                                VALUES (rowId
                                                                        , ProjectCode
                                                                        , Project
                                                                        , GETDATE()
                                                                        , @ImportUser);

                                        MERGE INTO Communities AS Target
                                        USING (SELECT
                                                    ImportId AS rowId,
                                                    LTRIM(RTRIM(Comm_Num)) AS communityNumber,
                                                    LTRIM(RTRIM(Community)) AS community,
                                                    (SELECT TOP 1 CityId 
                                                        FROM Cities C 
                                                        WHERE LTRIM(RTRIM(C.CityCode)) = LTRIM(RTRIM(MCI.CityCode))) AS cityId,
                                                    (SELECT TOP 1 DivisionId 
                                                        FROM Divisions C 
                                                        WHERE LTRIM(RTRIM(C.DivisionCode)) = LTRIM(RTRIM(MCI.DivCode))) AS divisionId,
                                                    (SELECT TOP 1 ProjectId 
                                                        FROM Projects C 
                                                        WHERE LTRIM(RTRIM(C.ProjectNumber)) = LTRIM(RTRIM(MCI.ProjectCode))) AS projectId,
                                                    (SELECT TOP 1 CityId 
                                                        FROM Cities C 
                                                        WHERE LTRIM(RTRIM(C.CityCode)) = LTRIM(RTRIM(MCI.SateliteCode))) AS sateliteId,
                                                    LTRIM(RTRIM(StatusCode)) AS communityStatus,
                                                    LTRIM(RTRIM(StatusDescription)) AS statusDescription,
                                                    LTRIM(RTRIM(TypeCC)) AS communityType,
                                                    LTRIM(RTRIM(PlanTypeDesc)) AS typeDescription 
                                                FROM (SELECT
                                                                        MIN(RowId) AS ImportId,
                                                                        Comm_Num,
                                                                        Community,
                                                                        CityCode,
                                                                        City,
                                                                        DivCode,
                                                                        Division,
                                                                        ProjectCode,
                                                                        Project,
                                                                        SateliteCode,
                                                                        Satelite,
                                                                        StatusCode,
                                                                        StatusDescription,
                                                                        TypeCC,
                                                                        PlanTypeDesc
                                                            FROM imports.CommunityStage
                                                            GROUP BY Comm_Num,
                                                                        Community,
                                                                        CityCode,
                                                                        City,
                                                                        DivCode,
                                                                        Division,
                                                                        ProjectCode,
                                                                        Project,
                                                                        SateliteCode,
                                                                        Satelite,
                                                                        StatusCode,
                                                                        StatusDescription,
                                                                        TypeCC,
                                                                        PlanTypeDesc) MCI) AS LIST
                                        ON TARGET.CommunityNumber = LIST.CommunityNumber
                                        WHEN NOT MATCHED THEN INSERT (CommunityId
                                                                        , CommunityNumber
                                                                        , CommunityName
                                                                        , CityId
                                                                        , DivisionId
                                                                        , ProjectId
                                                                        , SateliteCityId
                                                                        , CommunityStatusCode
                                                                        , CommunityStatusDescription
                                                                        , ProductType
                                                                        , ProductTypeDescription
                                                                        , CreatedDate
                                                                        , CreatedBy)
                                                                VALUES (rowId
                                                                        , communityNumber
                                                                        , community
                                                                        , cityid
                                                                        , divisionid
                                                                        , projectId
                                                                        , sateliteId
                                                                        , communityStatus
                                                                        , statusDescription
                                                                        , communityType
                                                                        , typeDescription
                                                                        , getdate()
                                                                        , @ImportUser)
                                        WHEN MATCHED THEN UPDATE SET TARGET.CityId = CASE WHEN List.CityId IS NOT NULL THEN LIST.CityId ELSE TARGET.CityId END,
                                                                        TARGET.DivisionId = LIST.DivisionId,
                                                                        TARGET.ProjectId = LIST.ProjectId,
                                                                        TARGET.SateliteCityId = CASE WHEN LIST.SateliteId IS NOT NULL THEN LIST.SateliteId ELSE TARGET.SateliteCityId END,
                                                                        TARGET.CommunityStatusCode = LIST.CommunityStatus,
                                                                        TARGET.CommunityStatusDescription = LIST.statusDescription,
                                                                        TARGET.ProductType = LIST.communityType,
                                                                        TARGET.ProductTypeDescription = LIST.typeDescription,
                                                                        TARGET.UpdatedDate = GETDATE(),
                                                                        TARGET.UPdatedBy = @ImportUser;
";


            using (var sc = new SqlConnection(connectionString))
            {
                sc.Open();

                using (var cmd = new SqlCommand("TRUNCATE TABLE imports.CommunityStage", sc))
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