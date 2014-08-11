CREATE TABLE WarrantyCalls(
    WarrantyCallId UNIQUEIDENTIFIER NOT NULL,
    WarrantyCallNumber INT,    
    WarrantyCallType VARCHAR(50),
    JobId UNIQUEIDENTIFIER,
    Contact VARCHAR(255),
    WarrantyRepresentativeEmployeeId UNIQUEIDENTIFIER,
    CompletionDate DATETIME2,
    WorkSummary VARCHAR(4000),
    HomeOwnerSignature VARCHAR(2000),
    CreatedDate DATETIME2,
    CreatedBy VARCHAR(255),
    UpdatedDate DATETIME2,
    UpdatedBy VARCHAR(255)
);

ALTER TABLE WarrantyCalls ADD CONSTRAINT DF_WarrantyCalls_Id
    DEFAULT NEWSEQUENTIALID() FOR WarrantyCallId;

ALTER TABLE WarrantyCalls ADD CONSTRAINT PK_WarrantyCalls
    PRIMARY KEY (WarrantyCallId); 

ALTER TABLE WarrantyCalls ADD CONSTRAINT FK_WarrantyCalls_JobId
    FOREIGN KEY (JobId) REFERENCES Jobs (JobId);

ALTER TABLE WarrantyCalls ADD CONSTRAINT FK_WarrantyCalls_WarrantyRepresentativeId
    FOREIGN KEY (WarrantyRepresentativeEmployeeId) REFERENCES Employees (EmployeeId);