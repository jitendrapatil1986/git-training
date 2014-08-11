CREATE TABLE WarrantyCalls(
    WarrantyCallId INT NOT NULL,
    WarrantyCallNumber INT,    
    WarrantyCallType VARCHAR(50),
    WarrantyCallStatusId TINYINT,
    JobId INT,
    Contact VARCHAR(255),
    WarrantyRepresentativeEmployeeId INT,
    CompletionDate DATETIME2,
    WorkSummary VARCHAR(4000),
    HomeOwnerSignature VARCHAR(2000),
    CreatedDate DATETIME2,
    CreatedBy VARCHAR(255),
    UpdatedDate DATETIME2,
    UpdatedBy VARCHAR(255),
    CONSTRAINT PK_WarrantyCalls
        PRIMARY KEY (WarrantyCallId),
    CONSTRAINT FK_WarrantyCalls_JobId
        FOREIGN KEY (JobId) REFERENCES Jobs (JobId),
    CONSTRAINT FK_WarrantyCalls_WarrantyRepresentativeId
        FOREIGN KEY (WarrantyRepresentativeEmployeeId) REFERENCES Employees (EmployeeId),
    CONSTRAINT FK_Warrantycalls_WarrantyCallStatusId
        FOREIGN KEY (WarrantyCallStatusId) REFERENCES lookups.WarrantyCallStatuses (WarrantyCallStatusId)
);