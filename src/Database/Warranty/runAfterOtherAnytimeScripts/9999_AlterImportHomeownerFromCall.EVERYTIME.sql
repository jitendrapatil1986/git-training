ALTER PROCEDURE [imports].[ImportHomeownerFromCall] @CityCodeList VARCHAR(4000) AS

DECLARE @ImportUser VARCHAR(255) = 'Lotus Import';

SELECT CommunityId INTO #CommunitiesToDelete
    FROM Communities 
    WHERE CityId IN (SELECT CityId FROM Cities WHERE ',' + @CityCodeList + ',' LIKE '%,' + CityCode + ',%' OR @CityCodeList IS NULL);

IF OBJECT_ID('tempdb..#OwnerStage') IS NOT NULL
    DROP TABLE #OwnerStage

CREATE TABLE #OwnerStage ( 
    id INT IDENTITY(1,1) PRIMARY KEY
    , jobId uniqueidentifier
    , maxDate datetime2
    , Homeowner varchar(256)
    , home_phone varchar(256)
    , other_phone varchar(256)
    , workphone_1 varchar(256)
    , workphone_2 varchar(256)
    , emailcontact varchar(256)
    , org_Homeowner varchar(256)
);

INSERT INTO #OwnerStage
SELECT j.JobId,
MAX(sci.Date_Open) AS Date_Open,
CASE WHEN LEN(LTRIM(RTRIM(sci.homeowner))) = 0 THEN 'NULL' ELSE '''' + REPLACE(LTRIM(RTRIM(sci.homeowner)),'''','''''') + '''' END as Homeowner,
CASE WHEN LEN(LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(REPLACE(Home_Phone, ')', ''), '(', ''), ' ', ''),'-','')))) = 0 THEN 'NULL' ELSE '''' + REPLACE(LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(REPLACE(Home_Phone, ')', ''), '(', ''), ' ', ''),'-',''))),'''','''''') + '''' END as home_phone,
CASE WHEN LEN(LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(REPLACE(other_phone, ')', ''), '(', ''), ' ', ''),'-','')))) = 0 THEN 'NULL' ELSE '''' + REPLACE(LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(REPLACE(other_phone, ')', ''), '(', ''), ' ', ''),'-',''))),'''','''''') + '''' END as other_phone,
CASE WHEN LEN(LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(REPLACE(workphone_1, ')', ''), '(', ''), ' ', ''),'-','')))) = 0 THEN 'NULL' ELSE '''' + REPLACE(LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(REPLACE(workphone_1, ')', ''), '(', ''), ' ', ''),'-',''))),'''','''''') + '''' END as workphone_1,
CASE WHEN LEN(LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(REPLACE(workphone_2, ')', ''), '(', ''), ' ', ''),'-','')))) = 0 THEN 'NULL' ELSE '''' + REPLACE(LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(REPLACE(workphone_2, ')', ''), '(', ''), ' ', ''),'-',''))),'''','''''') + '''' END as workphone_2,
CASE WHEN LEN(LTRIM(RTRIM(emailcontact))) = 0 THEN 'NULL' ELSE '''' + REPLACE(LTRIM(RTRIM(emailcontact)),'''','''''') + '''' END as emailcontact,
LOWER(LTRIM(RTRIM(sci.Homeowner))) AS Homeowner
FROM imports.ServiceCallImports SCI
INNER JOIN Jobs J ON
    J.JobNumber = SCI.Job_Num
    AND J.CommunityId IN (SELECT CommunityId FROM #CommunitiesToDelete)
GROUP BY j.JobId,
CASE WHEN LEN(LTRIM(RTRIM(sci.homeowner))) = 0 THEN 'NULL' ELSE '''' + REPLACE(LTRIM(RTRIM(sci.homeowner)),'''','''''') + '''' END ,
CASE WHEN LEN(LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(REPLACE(Home_Phone, ')', ''), '(', ''), ' ', ''),'-','')))) = 0 THEN 'NULL' ELSE '''' + REPLACE(LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(REPLACE(Home_Phone, ')', ''), '(', ''), ' ', ''),'-',''))),'''','''''') + '''' END,
CASE WHEN LEN(LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(REPLACE(other_phone, ')', ''), '(', ''), ' ', ''),'-','')))) = 0 THEN 'NULL' ELSE '''' + REPLACE(LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(REPLACE(other_phone, ')', ''), '(', ''), ' ', ''),'-',''))),'''','''''') + '''' END,
CASE WHEN LEN(LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(REPLACE(workphone_1, ')', ''), '(', ''), ' ', ''),'-','')))) = 0 THEN 'NULL' ELSE '''' + REPLACE(LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(REPLACE(workphone_1, ')', ''), '(', ''), ' ', ''),'-',''))),'''','''''') + '''' END,
CASE WHEN LEN(LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(REPLACE(workphone_2, ')', ''), '(', ''), ' ', ''),'-','')))) = 0 THEN 'NULL' ELSE '''' + REPLACE(LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(REPLACE(workphone_2, ')', ''), '(', ''), ' ', ''),'-',''))),'''','''''') + '''' END,
CASE WHEN LEN(LTRIM(RTRIM(emailcontact))) = 0 THEN 'NULL' ELSE '''' + REPLACE(LTRIM(RTRIM(emailcontact)),'''','''''') + '''' END,
LOWER(LTRIM(RTRIM(sci.Homeowner)))

DECLARE @id INT = (SELECT MIN(id) FROM #OwnerStage);

WHILE @id IS NOT NULL
BEGIN                
    DECLARE @STATEMENT NVARCHAR(MAX) = (SELECT CASE WHEN HomeownerId IS NOT NULL THEN
                                                    'UPDATE Homeowners SET '
                                                        + ' HomePhone = ' + Home_Phone
                                                        + ', OtherPhone = ' + Other_Phone
                                                        + ', WorkPhone1 = ' + WorkPhone_1
                                                        + ', WorkPhone2 = ' + WorkPhone_2
                                                        + ', EmailAddress = ' + EmailContact
                                                        + ', UpdatedBy = ''' + @ImportUser + ''''
                                                        + ', UpdatedDate = ''' + CONVERT(VARCHAR(256), GETDATE()) + ''''
                                                        + ' WHERE HomeownerId = ''' + CONVERT(VARCHAR(256), HomeownerId) + ''''
                                                ELSE 'INSERT INTO Homeowners (JobId
                                                                                , HomeOwnerNumber
                                                                                , HomeOwnerName
                                                                                , HomePhone
                                                                                , OtherPhone
                                                                                , WorkPhone1
                                                                                , WorkPhone2
                                                                                , EmailAddress
                                                                                , CreatedDate
                                                                                , CreatedBy)
                                                                        VALUES ( ''' + CONVERT(VARCHAR(256), os.jobId) + ''''
                                                                                + ', (ISNULL((SELECT MAX(HomeownerNumber)
                                                                                    FROM Homeowners WHERE JobId = ''' + CONVERT(VARCHAR(256), os.JobId) + '''),0) + 1)'
                                                                                + ', ' + homeowner
                                                                                + ', ' + Home_Phone
                                                                                + ', ' + other_phone
                                                                                + ', ' + workphone_1
                                                                                + ', ' + workphone_2
                                                                                + ', ' + emailcontact
                                                                                + ', ''' + CONVERT(VARCHAR(256), getdate()) + ''''
                                                                                + ', ''' + @ImportUser + ''');' END AS script
                                                FROM #OwnerStage os
                                                LEFT JOIN Homeowners h_owner ON
                                                    h_owner.JobId = os.JobId
                                                    and LOWER(LTRIM(RTRIM(h_owner.HomeownerName))) = os.org_Homeowner
                                                WHERE id = @id);
    exec sys.sp_executesql @STATEMENT;

    SET @id = (SELECT MIN(id) FROM #OwnerStage WHERE id > @id);
END

UPDATE j SET CurrentHomeownerId = (SELECT HomeownerId 
                                    FROM Homeowners o
                                    WHERE o.JobId = j.JobId
                                        AND o.HomeownerNumber = (SELECT MAX(o2.HomeownerNumber)
                                                                    FROM Homeowners o2
                                                                    WHERE o2.JobId = j.JobId)),
            UpdatedBy = @ImportUser,
            UpdatedDate = GETDATE()
FROM Jobs j
WHERE CommunityId IN (SELECT CommunityId FROM #CommunitiesToDelete);
