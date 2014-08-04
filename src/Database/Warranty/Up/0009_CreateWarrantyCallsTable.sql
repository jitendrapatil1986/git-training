CREATE TABLE WarrantyCalls(
    WarrantyCallId INT NOT NULL,
    WarrantyCallNumber INT,    
    WarrantyCallType VARCHAR(50),
    JobId INT,
    Contact VARCHAR(255),
    WarrantyRepresentativeId INT,
    CompletionDate DATETIME2,
    WorkSummary VARCHAR(4000),
    HomeOwnerSignature VARCHAR(2000),
    Comment VARCHAR(MAX),
    CreatedDate DATETIME2,
    CreatedBy VARCHAR(255),
    UpdatedDate DATETIME2,
    UpdatedBy VARCHAR(255)
);

ALTER TABLE WarrantyCalls ADD CONSTRAINT PK_WarrantyCalls
    PRIMARY KEY (WarrantyCallId); 

ALTER TABLE WarrantyCalls ADD CONSTRAINT FK_WarrantyCalls_JobId
    FOREIGN KEY (JobId) REFERENCES Jobs (JobId);

ALTER TABLE WarrantyCalls ADD CONSTRAINT FK_WarrantyCalls_WarrantyRepresentativeId
    FOREIGN KEY (WarrantyRepresentativeId) REFERENCES TeamMembers (TeamMemberId);