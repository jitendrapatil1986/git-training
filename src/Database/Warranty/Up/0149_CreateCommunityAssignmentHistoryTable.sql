CREATE TABLE CommunityAssignmentHistory (
    EmployeeAssignmentHistoryId UNIQUEIDENTIFIER NOT NULL CONSTRAINT
        DF_EmployeeAssignmentHistoryId_Id DEFAULT NEWSEQUENTIALID(),
    CommunityId UNIQUEIDENTIFIER,
	EmployeeId UNIQUEIDENTIFIER,
    CreatedBy VARCHAR(255),
	CreatedDate DATETIME2,
	AssignmentDate DATETIME2,
    CONSTRAINT PK_EmployeeAssignmentHistoryId
        PRIMARY KEY (EmployeeAssignmentHistoryId) 
);