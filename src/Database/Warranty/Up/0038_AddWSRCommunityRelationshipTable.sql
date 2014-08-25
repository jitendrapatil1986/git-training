CREATE TABLE lookups.Roles (
    RoleId TINYINT NOT NULL IDENTITY(1,1),
    RoleDescription VARCHAR(100),
    CONSTRAINT PK_Roles
        PRIMARY KEY (RoleId)
)

CREATE TABLE EmployeeAssignments(    
    EmployeeAssignmentId UNIQUEIDENTIFIER NOT NULL 
        CONSTRAINT DF_Payments_PaymentId DEFAULT NEWSEQUENTIALID(),
    CommunityId UNIQUEIDENTIFIER NOT NULL,
    EmployeeId UNIQUEIDENTIFIER NOT NULL,
    RoleId TINYINT NOT NULL,
    CONSTRAINT PK_CommunityWarrantyServiceRepresentatives
        PRIMARY KEY (EmployeeAssignmentId),
    CONSTRAINT FK_EmployeeAssignments_CommunityId
        FOREIGN KEY (CommunityId) REFERENCES Communities(CommunityId),
    CONSTRAINT FK_EmployeeAssignments_EmployeeId
        FOREIGN KEY (EmployeeId) REFERENCES Employees(EmployeeId),
    CONSTRAINT FK_EmployeeAssignments_RoleId
        FOREIGN KEY (RoleId) REFERENCES lookups.Roles(RoleId)
)