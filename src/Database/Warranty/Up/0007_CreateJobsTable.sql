CREATE TABLE Jobs (
    JobId UNIQUEIDENTIFIER NOT NULL,
    JobNumber VARCHAR(8),
    CloseDate DATE,
    AddressLine VARCHAR(255),
    City VARCHAR(100),
    StateCode CHAR(2),
    PostalCode VARCHAR(15),
    LegalDescription VARCHAR(100),    
    CommunityId UNIQUEIDENTIFIER,    
    CurrentHomeOwnerId UNIQUEIDENTIFIER,
    PlanType VARCHAR(3),
    PlanTypeDescription VARCHAR(20),
    PlanName VARCHAR(30),
    PlanNumber VARCHAR(4),
    Elevation VARCHAR(3),
    Swing VARCHAR(1),
    BuilderEmployeeId UNIQUEIDENTIFIER,    
    SalesConsultantEmployeeId UNIQUEIDENTIFIER,
    WarrantyExpirationDate DATE,
    TotalPrice DECIMAL(15,2),
    DoNotContact BIT,
    CreatedDate DATETIME2,
    CreatedBy VARCHAR(255),
    UpdatedDate DATETIME2,
    UpdatedBy VARCHAR(255)
);

ALTER TABLE Jobs ADD CONSTRAINT PK_Jobs
    PRIMARY KEY (JobId);

ALTER TABLE Jobs ADD CONSTRAINT FK_Jobs_CurrentOwnerId
    FOREIGN KEY (CurrentHomeOwnerId) REFERENCES HomeOwners(HomeOwnerId);

ALTER TABLE Jobs ADD CONSTRAINT FK_Jobs_CommunityId
    FOREIGN KEY (CommunityId) REFERENCES Communities(CommunityId);

ALTER TABLE Jobs ADD CONSTRAINT FK_Jobs_BuilderEmployeeId
    FOREIGN KEY (BuilderEmployeeId) REFERENCES Employees(EmployeeId);

ALTER TABLE Jobs ADD CONSTRAINT FK_Jobs_SalesConsultantEmployeeId
    FOREIGN KEY (SalesConsultantEmployeeId) REFERENCES Employees(EmployeeId);
