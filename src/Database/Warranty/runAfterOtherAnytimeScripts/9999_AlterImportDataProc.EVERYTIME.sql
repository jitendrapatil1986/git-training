ALTER PROCEDURE [imports].[ImportData] @CityCodeList VARCHAR(4000) AS

DECLARE @ImportUser VARCHAR(255) = 'Lotus Import';

IF EXISTS (SELECT 1 FROM sys.sysconstraints WHERE OBJECT_NAME(constid) = 'FK_HomeOwners_JobId')
    ALTER TABLE HomeOwners DROP CONSTRAINT FK_HomeOwners_JobId;

IF EXISTS (SELECT 1 FROM sys.sysconstraints WHERE OBJECT_NAME(constid) = 'FK_Jobs_CurrentOwnerId')
    ALTER TABLE Jobs DROP CONSTRAINT FK_Jobs_CurrentOwnerId;

IF EXISTS (SELECT 1 FROM sys.sysconstraints WHERE OBJECT_NAME(constid) = 'FK_JobOptions')
    ALTER TABLE JobOptions DROP CONSTRAINT FK_JobOptions;

IF EXISTS (SELECT 1 FROM sys.sysconstraints WHERE OBJECT_NAME(constid) = 'FK_ServiceCallLineItems_ServiceCallId')
    ALTER TABLE ServiceCallLineItems DROP CONSTRAINT FK_ServiceCallLineItems_ServiceCallId;

IF EXISTS (SELECT 1 FROM sys.sysconstraints WHERE OBJECT_NAME(constid) = 'FK_ServiceCallNotes_ServiceCallId')
    ALTER TABLE ServiceCallNotes DROP CONSTRAINT FK_ServiceCallNotes_ServiceCallId;

IF EXISTS (SELECT 1 FROM sys.sysconstraints WHERE OBJECT_NAME(constid) = 'FK_ServiceCalls_JobId')
    ALTER TABLE ServiceCalls DROP CONSTRAINT FK_ServiceCalls_JobId;

/*Import clean up*/
DELETE FROM imports.ServiceCallImports WHERE EXISTS (SELECT 1 FROM imports.ServiceCallImports Sub
                                                            WHERE imports.ServiceCallImports.Job_Num = Sub.Job_Num
                                                                    AND imports.ServiceCallImports.Call_Num = Sub.Call_Num
                                                                    AND imports.ServiceCallImports.Last_Modified < Sub.Last_Modified)

SELECT CommunityId INTO #CommunitiesToDelete
    FROM Communities 
    WHERE CityId IN (SELECT CityId FROM Cities WHERE ',' + @CityCodeList + ',' LIKE '%,' + CityCode + ',%' OR @CityCodeList IS NULL);

MERGE INTO Divisions AS TARGET
USING (SELECT
                importId AS rowId,
                DivisionCode,
                Division
            FROM (SELECT
                        ROW_NUMBER() OVER (PARTITION BY RTRIM(LTRIM(DivisionCode)) ORDER BY MIN(ImportId) DESC) AS rowNumber,
                        MIN(ImportId) AS ImportId,
                        RTRIM(LTRIM(DivisionCode)) AS DivisionCode,
                        RTRIM(LTRIM(Division)) AS Division
                    FROM imports.MasterCommunityImports
                    WHERE RTRIM(LTRIM(DivisionCode)) != ''
                    GROUP BY RTRIM(LTRIM(DivisionCode)), RTRIM(LTRIM(Division))) AS Div
            WHERE rowNumber = 1
                    ) AS LIST
ON TARGET.DivisionCode = LIST.DivisionCode
WHEN NOT MATCHED BY TARGET THEN INSERT (DivisionId
                                , DivisionCode
                                , DivisionName
                                , CreatedDate
                                , CreatedBy)
                        VALUES (rowId
                                , DivisionCode
                                , Division
                                , GETDATE()
                                , @ImportUser);

MERGE INTO Cities AS TARGET
USING (SELECT
            ImportId AS rowId,
            cityCode,
            city
        FROM (SELECT 
                    MIN(ImportId) AS ImportId,
                    LTRIM(RTRIM(CityCode)) AS CityCode,
                    LTRIM(RTRIM(City)) AS City
                FROM imports.MasterCommunityImports
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
                ROW_NUMBER() OVER (PARTITION BY LTRIM(RTRIM(ProjectCode)) ORDER BY MIN(ImportId) DESC) AS rowNumber,
                MIN(ImportId) AS ImportId,
                LTRIM(RTRIM(ProjectCode)) AS ProjectCode,
                LTRIM(RTRIM(Project)) Project 
                FROM imports.MasterCommunityImports
                WHERE LTRIM(RTRIM(ProjectCode)) != ''
                GROUP BY LTRIM(RTRIM(ProjectCode)), LTRIM(RTRIM(Project))) AS Proj
        WHERE rowNumber = 1) AS LIST
ON TARGET.ProjectNumber = LIST.ProjectCode
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
            LTRIM(RTRIM(CommunityNumber)) AS communityNumber,
            LTRIM(RTRIM(Community)) AS community,
            (SELECT TOP 1 CityId 
                FROM Cities C 
                WHERE LTRIM(RTRIM(C.CityCode)) = LTRIM(RTRIM(MCI.CityCode))) AS cityId,
            (SELECT TOP 1 DivisionId 
                FROM Divisions C 
                WHERE LTRIM(RTRIM(C.DivisionCode)) = LTRIM(RTRIM(MCI.DivisionCode))) AS divisionId,
            (SELECT TOP 1 ProjectId 
                FROM Projects C 
                WHERE LTRIM(RTRIM(C.ProjectNumber)) = LTRIM(RTRIM(MCI.ProjectCode))) AS projectId,
            (SELECT TOP 1 CityId 
                FROM Cities C 
                WHERE LTRIM(RTRIM(C.CityCode)) = LTRIM(RTRIM(MCI.SateliteCode))) AS sateliteId,
            LTRIM(RTRIM(CommunityStatus)) AS communityStatus,
            LTRIM(RTRIM(StatusDescription)) AS statusDescription,
            LTRIM(RTRIM(TypeCC)) AS communityType,
            LTRIM(RTRIM(PlanTypeDesc)) AS typeDescription 
        FROM (SELECT
                                MIN(ImportId) AS ImportId,
                                CommunityNumber,
                                Community,
                                CityCode,
                                City,
                                DivisionCode,
                                Division,
                                ProjectCode,
                                Project,
                                SateliteCode,
                                Satelite,
                                CommunityStatus,
                                StatusDescription,
                                TypeCC,
                                PlanTypeDesc
                    FROM imports.MasterCommunityImports
                    GROUP BY CommunityNumber,
                                Community,
                                CityCode,
                                City,
                                DivisionCode,
                                Division,
                                ProjectCode,
                                Project,
                                SateliteCode,
                                Satelite,
                                CommunityStatus,
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

MERGE INTO Employees AS TARGET
USING (SELECT
        CASE WHEN Number LIKE '99%' AND LEN(Number) > 6 
            THEN SUBSTRING(Number,3, LEN(Number)) 
            ELSE Number END as Number,
        MAX(Name) as Name
        FROM (
            SELECT 
                BuilderEmp_Num AS Number
                , Builder AS Name 
            FROM imports.ServiceCallImports
            GROUP BY BuilderEmp_Num, Builder
            
            UNION

            SELECT 
                SalesEmp_Num
                , Sales_Consultant
            FROM imports.ServiceCallImports
            GROUP BY SalesEmp_Num, Sales_Consultant

            UNION
            
            SELECT 
                BuilderEmployeeNumber
                , Builder
            FROM imports.CustomerImports
            GROUP BY BuilderEmployeeNumber, Builder

            UNION
            
            SELECT 
                SalesConsultantNumber
                , SalesConsultant 
            FROM imports.CustomerImports
            GROUP BY SalesConsultantNumber, SalesConsultant 

            UNION
            
            SELECT 
                WsrEmp_Num
                , Assigned_To 
            FROM imports.ServiceCallImports
            GROUP BY WsrEmp_Num, Assigned_To) Members
        WHERE 
            Number != '' 
            AND Number IS NOT NULL
            AND Name != '' 
            AND Name IS NOT NULL
        GROUP BY Number) AS LIST
ON EmployeeNumber = Number
WHEN NOT MATCHED THEN INSERT(EmployeeNumber
                             , EmployeeName
                             , CreatedDate
                             , CreatedBy)
                    VALUES(Number
                             , Name
                             , getdate()
                             , @ImportUser);

;WITH JobSet AS ((SELECT * 
                FROM Jobs
                WHERE CommunityId IN (SELECT CommunityId FROM #CommunitiesToDelete)))
MERGE INTO JobSet AS TARGET
USING (SELECT * FROM (SELECT
            ImportId AS rowId,
            JobNumber,
            CloseDate,
            JobAddress,
            City,
            StateCode,
            ZipCode,
            LegalDescription,
            (SELECT TOP 1 communityId 
                FROM Communities C 
                WHERE C.CommunityNumber = CI.CommunityNumber) AS CommunityId,
            PlanType,
            PlanTypeDescription,
            PlanName,
            JobPlan,
            Elevation,
            Swing,    
            (SELECT TOP 1 EmployeeId 
                FROM Employees 
                WHERE EmployeeNumber = CASE WHEN BuilderEmployeeNumber LIKE '99%' AND LEN(BuilderEmployeeNumber) > 6 
                                        THEN SUBSTRING(BuilderEmployeeNumber,3, LEN(BuilderEmployeeNumber)) 
                                        ELSE BuilderEmployeeNumber END
            ) AS Builder,
            (SELECT TOP 1 EmployeeId 
                FROM Employees 
                WHERE EmployeeNumber = CASE WHEN SalesConsultantNumber LIKE '99%' AND LEN(SalesConsultantNumber) > 6 
                                        THEN SUBSTRING(SalesConsultantNumber,3, LEN(SalesConsultantNumber)) 
                                        ELSE SalesConsultantNumber END
            ) AS Sales,
            WarrantyExpirationDate,
            ROW_NUMBER() OVER (PARTITION BY JobNumber ORDER BY ImportId DESC) AS rowNumber
        FROM imports.CustomerImports CI) S WHERE rowNumber = 1) AS LIST
ON TARGET.JobNumber = LIST.JobNumber
WHEN NOT MATCHED BY TARGET THEN INSERT (JobId
                                , JobNumber
                                , CloseDate
                                , AddressLine
                                , City
                                , StateCode
                                , PostalCode
                                , LegalDescription
                                , CommunityId
                                , PlanType
                                , PlanTypeDescription
                                , PlanName
                                , PlanNumber
                                , Elevation
                                , Swing
                                , BuilderEmployeeId
                                , SalesConsultantEmployeeId
                                , WarrantyExpirationDate
                                , CreatedDate
                                , CreatedBy
                                , JdeIdentifier)
                        VALUES (rowId
                                , JobNumber
                                , CloseDate
                                , JobAddress
                                , City
                                , StateCode
                                , ZipCode
                                , LegalDescription
                                , CommunityId
                                , PlanType
                                , PlanTypeDescription
                                , PlanName
                                , JobPlan
                                , Elevation
                                , Swing
                                , Builder
                                , Sales
                                , WarrantyExpirationDate
                                , GETDATE()
                                , @ImportUser
                                , JobNumber)
WHEN MATCHED AND (Target.UpdatedBy IS NULL OR Target.UpdatedBy = @ImportUser) THEN UPDATE SET 
                    TARGET.CloseDate = LIST.CloseDate,
                    TARGET.AddressLine = LIST.JobAddress,
                    TARGET.City = LIST.City,
                    TARGET.StateCode = LIST.StateCode,
                    TARGET.PostalCode = LIST.ZipCode,
                    TARGET.LegalDescription = LIST.LegalDescription,
                    TARGET.CommunityId = LIST.CommunityId,
                    TARGET.PlanType = LIST.PlanType,
                    TARGET.PlanTypeDescription = LIST.PlanTypeDescription,
                    TARGET.PlanName = LIST.PlanName,
                    TARGET.PlanNumber = LIST.JobPlan,
                    TARGET.Elevation = LIST.Elevation,
                    TARGET.Swing = LIST.Swing,
                    TARGET.BuilderEmployeeId = LIST.Builder,
                    TARGET.SalesConsultantEmployeeId = LIST.Sales,
                    TARGET.WarrantyExpirationDate = LIST.WarrantyExpirationDate,
                    TARGET.UpdatedDate = GETDATE(),
                    TARGET.UpdatedBy = @ImportUser;

;WITH JobSet AS ((SELECT * 
                FROM Jobs
                WHERE CommunityId IN (SELECT CommunityId FROM #CommunitiesToDelete)))
, OwnerSet AS (SELECT HO.* 
                FROM HomeOwners HO
                INNER JOIN JobSet JS ON
                    HO.JobId = JS.JobId)
MERGE INTO OwnerSet AS TARGET
USING (SELECT * FROM (SELECT
            ImportId AS rowId,
            (SELECT TOP 1 JobId FROM Jobs J WHERE J.JobNumber = CI.JobNumber ORDER BY JobId) AS jobId,
            1 AS IsCurrent,
            OwnerNumber,
            HomeOwner,
            HomePhone,
            OtherPhone,
            WorkPhone1,
            WorkPhone2,
            Emailcontact,
            ROW_NUMBER() OVER (PARTITION BY (SELECT TOP 1 JobId FROM Jobs J WHERE J.JobNumber = CI.JobNumber ORDER BY JobId), OwnerNumber ORDER BY ImportId DESC) as rowNumber
            FROM imports.CustomerImports CI
            WHERE EXISTS (SELECT TOP 1 JobId FROM Jobs J WHERE J.JobNumber = CI.JobNumber ORDER BY JobId)) s WHERE rowNumber = 1) AS LIST
ON TARGET.JobId = LIST.JobId AND LOWER(LTRIM(RTRIM(TARGET.HomeOwnerName))) = LOWER(LTRIM(RTRIM(LIST.Homeowner)))
WHEN NOT MATCHED BY TARGET THEN INSERT (HomeOwnerId
                                , JobId
                                , HomeOwnerNumber
                                , HomeOwnerName
                                , HomePhone
                                , OtherPhone
                                , WorkPhone1
                                , WorkPhone2
                                , EmailAddress
                                , CreatedDate
                                , CreatedBy)
                        VALUES (rowId
                                , jobId
                                , ownerNumber
                                , homeowner
                                , homephone
                                , otherphone
                                , workphone1
                                , workphone2
                                , emailcontact
                                , getdate()
                                , @ImportUser)
WHEN MATCHED THEN UPDATE SET TARGET.HomeOwnerNumber = LIST.ownerNumber,
                    TARGET.HomeOwnerName = LIST.HomeOwner,
                    TARGET.HomePhone = LIST.HomePhone,
                    TARGET.OtherPhone = LIST.OtherPhone,
                    TARGET.WorkPhone1 = LIST.WorkPhone1,
                    TARGET.WorkPhone2 = LIST.WorkPhone2,
                    TARGET.EmailAddress = LIST.EmailContact,
                    TARGET.UpdatedDate = GETDATE(),
                    TARGET.UpdatedBy = @ImportUser;

UPDATE Jobs SET CurrentHomeOwnerId = (SELECT TOP 1 HomeOwnerId 
                                        FROM HomeOwners HO 
                                        WHERE HO.JobId = Jobs.JobId
                                        ORDER BY HO.HomeOwnerNumber DESC)
WHERE (UpdatedBy IS NULL OR UpdatedBy = @ImportUser);

UPDATE I SET importid = ServiceCallId
FROM imports.ServiceCallImports I
    INNER JOIN ServiceCalls C ON
        C.ServiceCallNumber = I.Call_Num
    INNER JOIN Jobs J ON
        C.JobId = J.JobId
        AND J.JobNumber = I.Job_Num;

;WITH JobSet AS (SELECT * 
                FROM Jobs
                WHERE CommunityId IN (SELECT CommunityId FROM #CommunitiesToDelete)),
CallSet AS (SELECT SC.*
                   FROM ServiceCalls SC 
                   INNER JOIN JobSet JS ON
                        SC.JobId = JS.JobId)
MERGE INTO CallSet AS TARGET
USING (SELECT DISTINCT
            importid AS rowId,
            Call_Num,
            CallType,
            J.JobId AS jobId,
            Contact,
            (SELECT TOP 1 EmployeeId 
                FROM Employees 
                WHERE EmployeeNumber = CASE WHEN WsrEmp_Num LIKE '99%' AND LEN(WsrEmp_Num) > 6 
                                        THEN SUBSTRING(WsrEmp_Num,3, LEN(WsrEmp_Num)) 
                                        ELSE WsrEmp_Num END
                ) AS RepId,
            CASE WHEN Comp_Date = '' THEN null ELSE Comp_Date END AS Comp_Date,
            HOSig,
            Call_Comments,
            Date_Open ,
            'LI: ' + Assigned_By AS CreatedBy
        FROM imports.ServiceCallImports I
        INNER JOIN Jobs J ON
            I.Job_Num = J.JobNumber) AS LIST
ON TARGET.ServiceCallId = LIST.rowId
WHEN NOT MATCHED THEN INSERT (ServiceCallId
                                , ServiceCallNumber
                                , ServiceCallType
                                , JobId
                                , Contact
                                , WarrantyRepresentativeEmployeeId
                                , CompletionDate
                                , HomeOwnerSignature
                                , CreatedDate
                                , CreatedBy)
                        VALUES (rowId
                                , Call_Num
                                , CallType
                                , jobId
                                , Contact
                                , repId
                                , Comp_Date
                                , HOSig
                                , Date_Open
                                , CreatedBy)
WHEN MATCHED AND (Target.UpdatedBy IS NULL OR Target.UpdatedBy = @ImportUser) THEN UPDATE SET ServiceCallType = LIST.CallType
                                , JobId = LIST.JobId
                                , Contact = LIST.Contact
                                , WarrantyRepresentativeEmployeeId = LIST.repId
                                , CompletionDate = LIST.Comp_Date
                                , HomeOwnerSignature = LIST.HOSig
                                , CreatedDate = LIST.Date_Open
                                , CreatedBy = LIST.CreatedBy;


;WITH JobSet AS (SELECT * 
                FROM Jobs
                WHERE CommunityId IN (SELECT CommunityId FROM #CommunitiesToDelete)),
NoteSet AS (SELECT C.*
                   FROM ServiceCallNotes C
                   INNER JOIN ServiceCalls SC ON
                    C.ServiceCallId = SC.ServiceCallId
                   INNER JOIN JobSet JS ON
                    SC.JobId = JS.JobId OR SC.JobId IS NULL)
MERGE INTO NoteSet AS TARGET
USING (SELECT
            importId AS callId,
            Call_Comments,
            Date_Open,
            'LI: ' + R.Assigned_By AS CreatedBy            
        FROM imports.ServiceCallImports R
        INNER JOIN ServiceCalls SC ON
            R.importId = SC.ServiceCallId) AS LIST
ON TARGET.ServiceCallId = LIST.callId
    AND TARGET.ServiceCallNote =  LIST.Call_Comments
WHEN NOT MATCHED THEN INSERT (ServiceCallId,
                                ServiceCallNote,
                                CreatedDate,
                                CreatedBy)
                        VALUES(callId,
                                Call_Comments,
                                Date_Open,
                                CreatedBy);

;WITH JobSet AS (SELECT * 
                FROM Jobs
                WHERE CommunityId IN (SELECT CommunityId FROM #CommunitiesToDelete)),
LineItemSet AS (SELECT LI.*
                   FROM ServiceCallLineItems LI
                   INNER JOIN ServiceCalls SC ON
                    LI.ServiceCallId = SC.ServiceCallId
                   INNER JOIN JobSet JS ON
                    SC.JobId = JS.JobId OR SC.JobId IS NULL)
MERGE INTO LineItemSet AS TARGET
USING (SELECT
            importId AS callId,
            Items.LineNumber,
            Items.Code,
            PC.JdeCode AS ProblemJDECode,
            Items.Descr,
            Items.Cause,
            Items.ClassNote,
            Items.LineRoot,
            Items.Closed,
            Date_Open,
            'LI: ' + Assigned_By AS CreatedBy            
        FROM (
                SELECT  1 AS LineNumber,
                        PCode_1 AS Code,
                        Descript_1 AS Descr,
                        Cause_1 AS Cause,                
                        ResCode_1 AS ClassNote,                
                        Root_1 AS LineRoot,
                        CASE WHEN CDate_1 = 'Yes' THEN 1 ELSE 0 END AS Closed,
                        importId,
                        Date_Open,
                        Assigned_By
                    FROM imports.ServiceCallImports I
                    WHERE LTRIM(RTRIM(PCode_1)) != ''

                UNION ALL 

                SELECT  2 AS LineNumber,
                        PCode_2 AS Code,
                        Descript_2 AS Descr,
                        Cause_2 AS Cause,                
                        ResCode_2 AS ClassNote,                
                        Root_2 AS LineRoot,                        
                        CASE WHEN CDate_2 = 'Yes' THEN 1 ELSE 0 END AS Closed,
                        importId,
                        Date_Open,
                        Assigned_By
                    FROM imports.ServiceCallImports I
                    WHERE LTRIM(RTRIM(PCode_2)) != ''

                UNION ALL 

                SELECT  3 AS LineNumber,
                        PCode_3 AS Code,
                        Descript_3 AS Descr,
                        Cause_3 AS Cause,                
                        ResCode_3 AS ClassNote,                
                        Root_3 AS LineRoot,                        
                        CASE WHEN CDate_3 = 'Yes' THEN 1 ELSE 0 END AS Closed,
                        importId,
                        Date_Open,
                        Assigned_By
                    FROM imports.ServiceCallImports I
                    WHERE LTRIM(RTRIM(PCode_3)) != ''
                    
                UNION ALL 

                SELECT  4 AS LineNumber,
                        PCode_4 AS Code,
                        Descript_4 AS Descr,
                        Cause_4 AS Cause,                
                        ResCode_4 AS ClassNote,                
                        Root_4 AS LineRoot,
                        CASE WHEN CDate_4 = 'Yes' THEN 1 ELSE 0 END AS Closed,
                        importId,
                        Date_Open,
                        Assigned_By
                    FROM imports.ServiceCallImports I
                    WHERE LTRIM(RTRIM(PCode_4)) != ''
                    
                UNION ALL 

                SELECT  5 AS LineNumber,
                        PCode_5 AS Code,
                        Descript_5 AS Descr,
                        Cause_5 AS Cause,                
                        ResCode_5 AS ClassNote,                
                        Root_5 AS LineRoot,
                        CASE WHEN CDate_5 = 'Yes' THEN 1 ELSE 0 END AS Closed,
                        importId,
                        Date_Open,
                        Assigned_By
                    FROM imports.ServiceCallImports I
                    WHERE LTRIM(RTRIM(PCode_5)) != ''
        
                UNION ALL 

                SELECT  6 AS LineNumber,
                        PCode_6 AS Code,
                        Descript_6 AS Descr,
                        Cause_6 AS Cause,                
                        ResCode_6 AS ClassNote,                
                        Root_6 AS LineRoot,
                        CASE WHEN CDate_6 = 'Yes' THEN 1 ELSE 0 END AS Closed,
                        importId,
                        Date_Open,
                        Assigned_By
                    FROM imports.ServiceCallImports I
                    WHERE LTRIM(RTRIM(PCode_6)) != ''

                UNION ALL 

                SELECT  7 AS LineNumber,
                        PCode_7 AS Code,
                        Descript_7 AS Descr,
                        Cause_7 AS Cause,                
                        ResCode_7 AS ClassNote,                
                        Root_7 AS LineRoot,
                        CASE WHEN CDate_7 = 'Yes' THEN 1 ELSE 0 END AS Closed,
                        importId,
                        Date_Open,
                        Assigned_By
                    FROM imports.ServiceCallImports I
                    WHERE LTRIM(RTRIM(PCode_7)) != ''
            
                UNION ALL 

                SELECT  8 AS LineNumber,
                        PCode_8 AS Code,
                        Descript_8 AS Descr,
                        Cause_8 AS Cause,                
                        ResCode_8 AS ClassNote,                
                        Root_8 AS LineRoot,
                        CASE WHEN CDate_8 = 'Yes' THEN 1 ELSE 0 END AS Closed,
                        importId,
                        Date_Open,
                        Assigned_By
                    FROM imports.ServiceCallImports I
                    WHERE LTRIM(RTRIM(PCode_8)) != ''
            
                UNION ALL 

                SELECT  9 AS LineNumber,
                        PCode_9 AS Code,
                        Descript_9 AS Descr,
                        Cause_9 AS Cause,                
                        ResCode_9 AS ClassNote,                
                        Root_9 AS LineRoot,
                        CASE WHEN CDate_9 = 'Yes' THEN 1 ELSE 0 END AS Closed,
                        importId,
                        Date_Open,
                        Assigned_By
                    FROM imports.ServiceCallImports I
                    WHERE LTRIM(RTRIM(PCode_9)) != ''
                     
                UNION ALL 

                SELECT  10 AS LineNumber,
                        PCode_10 AS Code,
                        Descript_10 AS Descr,
                        Cause_10 AS Cause,                
                        ResCode_10 AS ClassNote,                
                        Root_10 AS LineRoot,
                        CASE WHEN CDate_10 = 'Yes' THEN 1 ELSE 0 END AS Closed,
                        importId,
                        Date_Open,
                        Assigned_By
                    FROM imports.ServiceCallImports I
                    WHERE LTRIM(RTRIM(PCode_10)) != ''
                    ) Items 
                    INNER JOIN ServiceCalls ON
                        Items.importId = ServiceCallId
                    LEFT JOIN (SELECT DISTINCT JdeCode, CategoryCode FROM ProblemCodes) PC ON
                        Items.Code = PC.CategoryCode) AS LIST
ON TARGET.ServiceCallId = LIST.callId 
    AND TARGET.LineNumber = LIST.LineNumber
WHEN NOT MATCHED THEN INSERT (ServiceCallId
                                , LineNumber
                                , ProblemCode
                                , ProblemDescription
                                , CauseDescription
                                , ClassificationNote
                                , LineItemRoot
                                , CreatedDate
                                , CreatedBy
                                , ServiceCallLineItemStatusId
                                , ProblemJDECode
                                , ProblemDetailCode)
                        VALUES (callId
                                , LineNumber
                                , Code
                                , Descr
                                , Cause
                                , ClassNote
                                , LineRoot
                                , Date_Open
                                , CreatedBy
                                , case when Closed = 0 then 1 else 3 end /*Service Call Line Item Status*/
                                , ProblemJDECode
                                , LineRoot)
WHEN MATCHED AND (Target.UpdatedBy IS NULL OR Target.UpdatedBy = @ImportUser) THEN UPDATE SET ProblemCode = Code,
                ProblemDescription = Descr,
                CauseDescription = Cause,
                ClassificationNote = ClassNote,
                LineItemRoot = LineRoot,
                UpdatedDate = GETDATE(),
                UpdatedBy = @ImportUser,
                ServiceCallLineItemStatusId = case when Closed = 0 then 1 else 3 end /*Service Call Line Item Status*/,
                ProblemJDECode = LIST.ProblemJDECode,
                ProblemDetailCode = LineRoot;

UPDATE ServiceCallLineItems SET ProblemCode = 
    CASE 
    WHEN ProblemCode = 'Caulking' THEN 'Caulk'
    WHEN ProblemCode = 'Garage Doors/Opener' THEN 'Garage Door/Opener'
    WHEN ProblemCode = 'Mirrors/Shower Doors' THEN 'Mirrors/Shower Door'
    ELSE ProblemCode END
WHERE ProblemCode IN ('Caulking','Garage Doors/Opener','Mirrors/Shower Doors')

UPDATE ServiceCallLineItems SET ProblemJDECode = JdeCode
FROM (SELECT DISTINCT CategoryCode, Jdecode FROM ProblemCodes) AS PC
WHERE CategoryCode = ProblemCode AND ProblemJDECode IS NULL;

;WITH JobSet AS (SELECT * 
                FROM Jobs
                WHERE CommunityId IN (SELECT CommunityId FROM #CommunitiesToDelete)),
OptionSet AS (SELECT JO.*
                FROM JobOptions JO
                INNER JOIN JobSet JS ON
                    JO.JobId = JS.JobId)
MERGE INTO OptionSet AS TARGET
USING (SELECT
            rowId,
            (SELECT TOP 1 JobId 
                FROM Jobs J 
                WHERE J.JobNumber = I.JobNumber) AS jobId,
            OptionNumber,
            OptionDescription,
            Quantity
        FROM
            (SELECT MIN(ImportId) AS rowId,
                            JobNumber,
                            OptionNumber,
                            OptionDescription,
                            Quantity
            FROM imports.JobOptionImports
            GROUP BY JobNumber,
                            OptionNumber,
                            OptionDescription,
                            Quantity) I) AS LIST
ON TARGET.JobId = LIST.JobId AND TARGET.OptionNumber = LIST.OptionNumber
WHEN NOT MATCHED AND LIST.JobId IS NOT NULL THEN INSERT (JobOptionId
                                , JobId
                                , OptionNumber
                                , OptionDescription
                                , Quantity
                                , CreatedDate
                                , CreatedBy)
                         VALUES (rowId
                                , jobId
                                , optionNumber
                                , optionDescription
                                , quantity
                                , GETDATE()
                                , @ImportUser)
WHEN MATCHED THEN UPDATE SET
    TARGET.OptionDescription = LIST.OptionDescription,
    TARGET.Quantity = LIST.Quantity;

UPDATE ServiceCalls SET ServiceCallStatusId = S.ServiceCallStatusId
FROM lookups.ServiceCallStatuses S
WHERE 
(ServiceCalls.UpdatedBy IS NULL OR ServiceCalls.UpdatedBy = @ImportUser) 
AND
(
    ServiceCalls.ServiceCallStatusId IS NULL
    OR ServiceCalls.ServiceCallStatusId IN (SELECT ServiceCallStatusId
                                                FROM lookups.ServiceCallStatuses
                                                WHERE ServiceCallStatus IN ('Open', 'Closed'))
) AND
    (
        (
            CASE WHEN CompletionDate = '' THEN NULL ELSE CompletionDate END IS NULL
            AND S.ServiceCallStatus = 'Open'
        )
        OR
        (
            CASE WHEN CompletionDate = '' THEN NULL ELSE CompletionDate END IS NOT NULL
            AND S.ServiceCallStatus = 'Closed'
        )
    );
    
MERGE INTO CommunityAssignments AS TARGET
USING (SELECT DISTINCT c.CommunityId, MAX(e.EmployeeId) AS EmployeeId
        FROM Communities C 
        INNER JOIN imports.MasterCommunityImports MC ON
            C.CommunityNumber = MC.CommunityNumber
            AND mc.WarrantyServiceRepresentative != ''
        INNER JOIN Employees e ON
            LOWER(e.EmployeeName) LIKE '%'+ REPLACE(mc.WarrantyServiceRepresentative,' ', '%') +'%'         
GROUP BY c.communityid) AS SOURCE
ON TARGET.CommunityId = SOURCE.CommunityId
WHEN NOT MATCHED THEN INSERT (CommunityId, EmployeeId) VALUES (CommunityId, EmployeeId);

ALTER TABLE HomeOwners ADD CONSTRAINT FK_HomeOwners_JobId
    FOREIGN KEY (JobId) REFERENCES Jobs(JobId);

ALTER TABLE Jobs ADD CONSTRAINT FK_Jobs_CurrentOwnerId 
    FOREIGN KEY(CurrentHomeOwnerId) REFERENCES HomeOwners(HomeOwnerId);

ALTER TABLE JobOptions ADD CONSTRAINT FK_JobOptions
    FOREIGN KEY(JobId) REFERENCES Jobs(JobId);

ALTER TABLE ServiceCalls ADD CONSTRAINT FK_ServiceCalls_JobId
    FOREIGN KEY(JobId) REFERENCES Jobs (JobId);

ALTER TABLE ServiceCallNotes WITH CHECK ADD CONSTRAINT FK_ServiceCallNotes_ServiceCallId 
    FOREIGN KEY(ServiceCallId) REFERENCES ServiceCalls (ServiceCallId);

ALTER TABLE ServiceCallLineItems  ADD  CONSTRAINT FK_ServiceCallLineItems_ServiceCallId 
    FOREIGN KEY(ServiceCallId) REFERENCES ServiceCalls (ServiceCallId);
