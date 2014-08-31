CREATE TABLE CommunityAssignments(    
    EmployeeAssignmentId UNIQUEIDENTIFIER NOT NULL 
        CONSTRAINT DF_CommunityAssignments_EmployeeAssignmentId DEFAULT NEWSEQUENTIALID(),
    CommunityId UNIQUEIDENTIFIER NOT NULL,
    EmployeeId UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT PK_CommunityWarrantyServiceRepresentatives
        PRIMARY KEY (EmployeeAssignmentId),
    CONSTRAINT FK_EmployeeAssignments_CommunityId
        FOREIGN KEY (CommunityId) REFERENCES Communities(CommunityId),
    CONSTRAINT FK_EmployeeAssignments_EmployeeId
        FOREIGN KEY (EmployeeId) REFERENCES Employees(EmployeeId)
)