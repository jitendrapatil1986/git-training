CREATE PROCEDURE imports.ImportData AS
DECLARE @ImportUser VARCHAR(255) = 'Lotus Import';

MERGE INTO Divisions AS Target
USING (SELECT
                ISNULL((SELECT MAX(divisionId) FROM Divisions), 0) + ROW_NUMBER() OVER (ORDER BY divisionCode) AS rowId,
                DivisionCode,
                Division
            FROM (SELECT DISTINCT
                        RTRIM(LTRIM(DivisionCode)) AS DivisionCode,
                        RTRIM(LTRIM(Division)) AS Division
                    FROM imports.MasterCommunityImports) AS Div) AS LIST
ON TARGET.DivisionCode = LIST.DivisionCode
    AND TARGET.DivisionName = LIST.Division
WHEN NOT MATCHED THEN INSERT (DivisionId
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
            ISNULL((SELECT MAX(cityId) FROM Cities), 0) + ROW_NUMBER() OVER (ORDER BY cityCode) AS rowId,
            cityCode,
            city
        FROM (SELECT DISTINCT 
                    LTRIM(RTRIM(CityCode)) AS CityCode,
                    LTRIM(RTRIM(City)) AS City
                FROM imports.MasterCommunityImports) AS City) AS LIST
ON TARGET.CityCode = LIST.CityCode
    AND TARGET.CityName = LIST.City
WHEN NOT MATCHED THEN INSERT (CityId
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
            ISNULL((SELECT MAX(projectId) FROM Projects),0) + ROW_NUMBER() OVER (ORDER BY projectCode) AS rowId,
            projectCode,
            project
        FROM (SELECT DISTINCT 
                LTRIM(RTRIM(ProjectCode)) AS ProjectCode,
                LTRIM(RTRIM(Project)) Project 
                FROM imports.MasterCommunityImports) AS Proj) AS LIST
ON TARGET.ProjectNumber = LIST.ProjectCode 
    AND TARGET.ProjectName = LIST.Project
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
            ISNULL((SELECT MAX(CommunityId) FROM Communities), 0) + ROW_NUMBER() OVER (ORDER BY CommunityNumber) AS rowId,
            LTRIM(RTRIM(CommunityNumber)) AS communityNumber,
            LTRIM(RTRIM(Community)) AS community,
            (SELECT TOP 1 CityId 
                FROM Cities C 
                WHERE LTRIM(RTRIM(C.CityCode)) = LTRIM(RTRIM(MCI.CityCode)) 
                AND LTRIM(RTRIM(C.CityName)) = LTRIM(RTRIM(MCI.City))) AS cityId,
            (SELECT TOP 1 DivisionId 
                FROM Divisions C 
                WHERE LTRIM(RTRIM(C.DivisionCode)) = LTRIM(RTRIM(MCI.DivisionCode)) 
                AND LTRIM(RTRIM(C.DivisionName)) = LTRIM(RTRIM(MCI.Division))) AS divisionId,
            (SELECT TOP 1 ProjectId 
                FROM Projects C 
                WHERE LTRIM(RTRIM(C.ProjectNumber)) = LTRIM(RTRIM(MCI.ProjectCode)) 
                AND LTRIM(RTRIM(C.ProjectName)) = LTRIM(RTRIM(MCI.Project))) AS projectId,
            (SELECT TOP 1 CityId 
                FROM Cities C 
                WHERE LTRIM(RTRIM(C.CityCode)) = LTRIM(RTRIM(MCI.SateliteCode)) 
                AND LTRIM(RTRIM(C.CityName)) = LTRIM(RTRIM(MCI.Satelite))) AS sateliteId,
            LTRIM(RTRIM(CommunityStatus)) AS communityStatus,
            LTRIM(RTRIM(StatusDescription)) AS statusDescription,
            LTRIM(RTRIM(TypeCC)) AS communityType,
            LTRIM(RTRIM(PlanTypeDesc)) AS typeDescription 
        FROM (SELECT DISTINCT CommunityNumber,
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
                    FROM imports.MasterCommunityImports) MCI) AS LIST
ON TARGET.CommunityNumber = LIST.CommunityNumber 
    AND TARGET.CommunityName = LIST.Community
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
WHEN MATCHED THEN UPDATE SET TARGET.CityId = LIST.CityId,
                                TARGET.DivisionId = LIST.DivisionId,
                                TARGET.ProjectId = LIST.ProjectId,
                                TARGET.SateliteCityId = LIST.SateliteId,
                                TARGET.CommunityStatusCode = LIST.CommunityStatus,
                                TARGET.CommunityStatusDescription = LIST.statusDescription,
                                TARGET.ProductType = LIST.communityType,
                                TARGET.ProductTypeDescription = LIST.typeDescription,
                                TARGET.UpdatedDate = GETDATE(),
                                TARGET.UPdatedBy = @ImportUser;

MERGE INTO Employees AS TARGET
USING (SELECT
        ISNULL((SELECT MAX(EmployeeId) FROM Employees), 0) + ROW_NUMBER() OVER (ORDER BY Number) AS rowId,
        Number,
        Name
        FROM (
            SELECT DISTINCT 
                BuilderEmp_Num AS Number
                , Builder AS Name 
            FROM imports.WarrantyCallImports
            
            UNION

            SELECT DISTINCT 
                SalesEmp_Num
                , Sales_Consultant
            FROM imports.WarrantyCallImports

            UNION
            
            SELECT DISTINCT 
                BuilderEmployeeNumber
                , Builder
            FROM imports.CustomerImports
            
            UNION
            
            SELECT DISTINCT 
                SalesConsultantNumber
                , SalesConsultant 
            FROM imports.CustomerImports
            
            UNION
            
            SELECT DISTINCT 
                WsrEmp_Num
                , Assigned_To 
            FROM imports.WarrantyCallImports ) Members
        WHERE 
            Number != '' 
            OR Name != '') AS LIST
ON EmployeeNumber = Number 
    AND EmployeeName = Name
WHEN NOT MATCHED THEN INSERT(EmployeeId
                             , EmployeeNumber
                             , EmployeeName
                             , CreatedDate
                             , CreatedBy)
                    VALUES(rowId
                             , Number
                             , Name
                             , getdate()
                             , @ImportUser);

MERGE INTO Jobs AS TARGET
USING (SELECT
            ISNULL((SELECT MAX(JobId) FROM Jobs),1) + ROW_NUMBER() OVER (ORDER BY JobNumber) AS rowId,
            JobNumber,
            CloseDate,
            JobAddress,
            City,
            StateCode,
            ZipCode,
            LegalDescription,
            (SELECT TOP 1 communityId 
                FROM Communities C 
                WHERE C.CommunityNumber = CI.CommunityNumber 
                AND C.CommunityName = CI.Community) AS CommunityId,
            PlanType,
            PlanTypeDescription,
            PlanName,
            JobPlan,
            Elevation,
            Swing,    
            (SELECT TOP 1 EmployeeId 
                FROM Employees 
                WHERE EmployeeNumber = BuilderEmployeeNumber 
                AND EmployeeName = Builder) AS Builder,
            (SELECT TOP 1 EmployeeId 
                FROM Employees 
                WHERE SalesConsultantNumber = BuilderEmployeeNumber 
                AND EmployeeName = SalesConsultant) AS Sales,
            WarrantyExpirationDate,
            TotalSalesPrice
        FROM imports.CustomerImports CI) AS LIST
ON TARGET.JobNumber = LIST.JobNumber
WHEN NOT MATCHED THEN INSERT (JobId
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
                                , TotalPrice
                                , CreatedDate
                                , CreatedBy)
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
                                , TotalSalesPrice
                                , GETDATE()
                                , @ImportUser)
WHEN MATCHED THEN UPDATE SET 
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
                    TARGET.TotalPrice = LIST.TotalSalesPrice,
                    TARGET.UpdatedDate = GETDATE(),
                    TARGET.UpdatedBy = @ImportUser;
        
MERGE INTO HomeOwners AS TARGET
USING (SELECT
            ISNULL((SELECT MAX(HomeOwnerId) FROM HomeOwners), 0) + ROW_NUMBER() OVER (ORDER BY CI.JobNumber) AS rowId,
            (SELECT TOP 1 JobId FROM Jobs J WHERE J.JobNumber = CI.JobNumber) AS jobId,
            1 AS IsCurrent,
            OwnerNumber,
            HomeOwner,
            HomePhone,
            OtherPhone,
            WorkPhone1,
            WorkPhone2,
            Emailcontact
            FROM imports.CustomerImports CI) AS LIST
ON TARGET.JobId = LIST.JobId AND TARGET.HomeOwnerNumber = LIST.OwnerNumber
WHEN NOT MATCHED THEN INSERT (HomeOwnerId
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
                                , @importUser)
WHEN MATCHED THEN UPDATE SET TARGET.HomeOwnerNumber = LIST.ownerNumber,
                    TARGET.HomeOwnerName = LIST.HomeOwner,
                    TARGET.HomePhone = LIST.HomePhone,
                    TARGET.OtherPhone = LIST.OtherPhone,
                    TARGET.WorkPhone1 = LIST.WorkPhone1,
                    TARGET.WorkPhone2 = LIST.WorkPhone2,
                    TARGET.EmailAddress = LIST.EmailContact,
                    TARGET.UpdatedDate = GETDATE(),
                    TARGET.UpdatedBy = @ImportUser;

UPDATE Jobs SET CurrentHomeOwnerId = (SELECT TOP 1 HomeOwnerId FROM HomeOwners HO WHERE HO.JobId = Jobs.JobId)

UPDATE I SET insertedRowId = WarrantyCallId
FROM imports.WarrantyCallImports I
    INNER JOIN WarrantyCalls C ON
        C.WarrantyCallNumber = I.Call_Num 
    AND C.WarrantyCallType = I.CallType    
    AND C.JobId = (SELECT TOP 1 JobId 
                FROM Jobs J 
                WHERE J.JobNumber = I.Job_Num) 
    AND C.Contact = I.Contact
    AND C.WarrantyRepresentativeEmployeeId = 
            (SELECT TOP 1 EmployeeId 
                FROM Employees 
                WHERE EmployeeNumber = WsrEmp_Num 
                AND EmployeeName = Assigned_To) 
    AND C.CompletionDate = I.Comp_Date
    AND C.HomeOwnerSignature = I.HOSig
    AND C.CreatedDate = I.Date_Open 
    AND C.CreatedBy = 'LI: ' + Assigned_By;

DECLARE @maxCallId INT = ISNULL((SELECT MAX(WarrantyCallId) FROM WarrantyCalls),0);

UPDATE imports.WarrantyCallImports SET insertedRowId = importId + @maxCallId
    WHERE insertedRowId IS NULL;

MERGE INTO WarrantyCalls AS TARGET
USING (SELECT
            insertedRowId AS rowId,
            Call_Num,
            CallType,
            (SELECT TOP 1 JobId 
                FROM Jobs J 
                WHERE J.JobNumber = I.Job_Num) AS jobId,
            Contact,
            (SELECT TOP 1 EmployeeId 
                FROM Employees 
                WHERE EmployeeNumber = WsrEmp_Num 
                AND EmployeeName = Assigned_To) AS RepId,
            Comp_Date,
            HOSig,
            Call_Comments,
            Date_Open ,
            'LI: ' + Assigned_By AS CreatedBy
        FROM imports.WarrantyCallImports I) AS LIST
ON TARGET.WarrantyCallNumber = LIST.Call_Num 
    AND TARGET.WarrantyCallType = CallType    
    AND TARGET.JobId = LIST.JobId 
    AND TARGET.Contact = LIST.Contact
    AND TARGET.WarrantyRepresentativeEmployeeId = RepId    
    AND TARGET.CompletionDate = LIST.Comp_Date
    AND TARGET.HomeOwnerSignature = LIST.HOSig
    AND TARGET.CreatedDate = LIST.Date_Open 
    AND TARGET.CreatedBy = LIST.CreatedBy
WHEN NOT MATCHED THEN INSERT (WarrantyCallId
                                , WarrantyCallNumber
                                , WarrantyCallType
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
                                , CreatedBy);

MERGE INTO WarrantyCallComments AS TARGET
USING (SELECT
            ISNULL((SELECT MAX(WarrantyCallCommentId) FROM WarrantyCallComments),0) + ROW_NUMBER() OVER (ORDER BY R.Call_Num) AS rowId,
            insertedRowId AS callId,
            Call_Comments,
            Date_Open,
            'LI: ' + R.Assigned_By AS CreatedBy            
        FROM imports.WarrantyCallImports R) AS LIST
ON TARGET.WarrantyCallId = LIST.callId
    AND TARGET.warrantyCallComment =  LIST.Call_Comments
WHEN NOT MATCHED THEN INSERT (WarrantyCallCommentId,
                                WarrantyCallId,
                                WarrantyCallComment,
                                CreatedDate,
                                CreatedBy)
                        VALUES(rowId,
                                callId,
                                Call_Comments,
                                Date_Open,
                                CreatedBy);

MERGE INTO WarrantyCallLineItems AS TARGET
USING (SELECT
            ISNULL((SELECT MAX(WarrantyCallLineItemId) FROM WarrantyCallLineItems),0) + ROW_NUMBER() OVER (ORDER BY insertedRowId) AS rowId,
            insertedRowId AS callId,
            Items.LineNumber,
            Items.Code,
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
                        insertedRowId,
                        Date_Open,
                        Assigned_By
                    FROM imports.WarrantyCallImports I

                UNION ALL 

                SELECT  2 AS LineNumber,
                        PCode_2 AS Code,
                        Descript_2 AS Descr,
                        Cause_2 AS Cause,                
                        ResCode_2 AS ClassNote,                
                        Root_2 AS LineRoot,                        
                        CASE WHEN CDate_2 = 'Yes' THEN 1 ELSE 0 END AS Closed,
                        insertedRowId,
                        Date_Open,
                        Assigned_By
                    FROM imports.WarrantyCallImports I

                UNION ALL 

                SELECT  3 AS LineNumber,
                        PCode_3 AS Code,
                        Descript_3 AS Descr,
                        Cause_3 AS Cause,                
                        ResCode_3 AS ClassNote,                
                        Root_3 AS LineRoot,                        
                        CASE WHEN CDate_3 = 'Yes' THEN 1 ELSE 0 END AS Closed,
                        insertedRowId,
                        Date_Open,
                        Assigned_By
                    FROM imports.WarrantyCallImports I
                    
                UNION ALL 

                SELECT  4 AS LineNumber,
                        PCode_4 AS Code,
                        Descript_4 AS Descr,
                        Cause_4 AS Cause,                
                        ResCode_4 AS ClassNote,                
                        Root_4 AS LineRoot,
                        CASE WHEN CDate_4 = 'Yes' THEN 1 ELSE 0 END AS Closed,
                        insertedRowId,
                        Date_Open,
                        Assigned_By
                    FROM imports.WarrantyCallImports I
                    
                UNION ALL 

                SELECT  5 AS LineNumber,
                        PCode_5 AS Code,
                        Descript_5 AS Descr,
                        Cause_5 AS Cause,                
                        ResCode_5 AS ClassNote,                
                        Root_5 AS LineRoot,
                        CASE WHEN CDate_5 = 'Yes' THEN 1 ELSE 0 END AS Closed,
                        insertedRowId,
                        Date_Open,
                        Assigned_By
                    FROM imports.WarrantyCallImports I
        
                UNION ALL 

                SELECT  6 AS LineNumber,
                        PCode_6 AS Code,
                        Descript_6 AS Descr,
                        Cause_6 AS Cause,                
                        ResCode_6 AS ClassNote,                
                        Root_6 AS LineRoot,
                        CASE WHEN CDate_6 = 'Yes' THEN 1 ELSE 0 END AS Closed,
                        insertedRowId,
                        Date_Open,
                        Assigned_By
                    FROM imports.WarrantyCallImports I

                UNION ALL 

                SELECT  7 AS LineNumber,
                        PCode_7 AS Code,
                        Descript_7 AS Descr,
                        Cause_7 AS Cause,                
                        ResCode_7 AS ClassNote,                
                        Root_7 AS LineRoot,
                        CASE WHEN CDate_7 = 'Yes' THEN 1 ELSE 0 END AS Closed,
                        insertedRowId,
                        Date_Open,
                        Assigned_By
                    FROM imports.WarrantyCallImports I
            
                UNION ALL 

                SELECT  8 AS LineNumber,
                        PCode_8 AS Code,
                        Descript_8 AS Descr,
                        Cause_8 AS Cause,                
                        ResCode_8 AS ClassNote,                
                        Root_8 AS LineRoot,
                        CASE WHEN CDate_8 = 'Yes' THEN 1 ELSE 0 END AS Closed,
                        insertedRowId,
                        Date_Open,
                        Assigned_By
                    FROM imports.WarrantyCallImports I
            
                UNION ALL 

                SELECT  9 AS LineNumber,
                        PCode_9 AS Code,
                        Descript_9 AS Descr,
                        Cause_9 AS Cause,                
                        ResCode_9 AS ClassNote,                
                        Root_9 AS LineRoot,
                        CASE WHEN CDate_9 = 'Yes' THEN 1 ELSE 0 END AS Closed,
                        insertedRowId,
                        Date_Open,
                        Assigned_By
                    FROM imports.WarrantyCallImports I
                     
                UNION ALL 

                SELECT  10 AS LineNumber,
                        PCode_10 AS Code,
                        Descript_10 AS Descr,
                        Cause_10 AS Cause,                
                        ResCode_10 AS ClassNote,                
                        Root_10 AS LineRoot,
                        CASE WHEN CDate_10 = 'Yes' THEN 1 ELSE 0 END AS Closed,
                        insertedRowId,
                        Date_Open,
                        Assigned_By
                    FROM imports.WarrantyCallImports I
                    ) Items ) AS LIST
ON TARGET.WarrantyCallId = LIST.callId 
    AND TARGET.LineNumber = LIST.LineNumber
WHEN NOT MATCHED THEN INSERT (WarrantyCallLineItemId
                                , WarrantyCallId
                                , LineNumber
                                , ProblemCode
                                , ProblemDescription
                                , CauseDescription
                                , ClassificationNote
                                , LineItemRoot
                                , Completed
                                , CreatedDate
                                , CreatedBy)
                        VALUES (rowId
                                , callId
                                , LineNumber
                                , Code
                                , Descr
                                , Cause
                                , ClassNote
                                , LineRoot
                                , Closed
                                , Date_Open
                                , CreatedBy)
WHEN MATCHED THEN UPDATE SET ProblemCode = Code,
                ProblemDescription = Descr,
                CauseDescription = Cause,
                ClassificationNote = ClassNote,
                LineItemRoot = LineRoot,
                Completed = Closed,
                UpdatedDate = GETDATE(),
                CreatedBy = @ImportUser;

MERGE INTO JobOptions AS TARGET
USING (SELECT
            ISNULL((SELECT MAX(JobOptionId) FROM JobOptions), 0) + ROW_NUMBER() OVER (ORDER BY JobNumber) AS rowId,
            (SELECT TOP 1 JobId 
                FROM Jobs J 
                WHERE J.JobNumber = I.JobNumber) AS jobId,
            OptionNumber,
            OptionDescription,
            Quantity
        FROM
            (SELECT DISTINCT JobNumber, OptionNumber, OptionDescription, Quantity FROM imports.JobOptionImports) I) AS LIST
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
