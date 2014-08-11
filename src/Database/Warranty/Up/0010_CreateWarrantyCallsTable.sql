CREATE TABLE ServiceCalls(
    ServiceCallId UNIQUEIDENTIFIER NOT NULL CONSTRAINT 
        DF_ServiceCalls_Id DEFAULT NEWSEQUENTIALID(),
    ServiceCallNumber INT,    
    ServiceCallType VARCHAR(50),
    ServiceCallStatusId TINYINT,
    JobId UNIQUEIDENTIFIER,
    Contact VARCHAR(255),
    WarrantyRepresentativeEmployeeId UNIQUEIDENTIFIER,
    CompletionDate DATETIME2,
    WorkSummary VARCHAR(4000),
    HomeOwnerSignature VARCHAR(2000),
    CreatedDate DATETIME2,
    CreatedBy VARCHAR(255),
    UpdatedDate DATETIME2,
    UpdatedBy VARCHAR(255),    
    CONSTRAINT PK_ServiceCalls
        PRIMARY KEY (ServiceCallId),
    CONSTRAINT FK_ServiceCalls_JobId
        FOREIGN KEY (JobId) REFERENCES Jobs (JobId),
    CONSTRAINT FK_ServiceCalls_WarrantyRepresentativeId
        FOREIGN KEY (WarrantyRepresentativeEmployeeId) REFERENCES Employees (EmployeeId),
    CONSTRAINT FK_Servicecalls_ServiceCallStatusId
        FOREIGN KEY (ServiceCallStatusId) REFERENCES lookups.ServiceCallStatuses (ServiceCallStatusId)
);