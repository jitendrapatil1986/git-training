CREATE TABLE CommunityAssignmentHistory (
    EmployeeAssignmentHistoryId UNIQUEIDENTIFIER NOT NULL CONSTRAINT
        DF_EmployeeAssignmentHistoryId_Id DEFAULT NEWSEQUENTIALID(),
    EmployeeAssignmentId UNIQUEIDENTIFIER,
    CommunityId UNIQUEIDENTIFIER,
	EmployeeId UNIQUEIDENTIFIER,
    CreatedDate DATETIME2,
    CreatedBy VARCHAR(255),
	AssignmentDate DATETIME2,
    CONSTRAINT PK_EmployeeAssignmentHistoryId
        PRIMARY KEY (EmployeeAssignmentHistoryId),
	CONSTRAINT FK_EmployeeAssigmentId FOREIGN KEY (EmployeeAssignmentId)
		REFERENCES CommunityAssignments(EmployeeAssignmentId) 
);