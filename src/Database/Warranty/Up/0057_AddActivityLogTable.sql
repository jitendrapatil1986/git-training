CREATE TABLE ActivityLog(
    ActivityLogId UNIQUEIDENTIFIER NOT NULL CONSTRAINT 
        DF_ActivityLog_Id DEFAULT NEWSEQUENTIALID(),
    ActivityName VARCHAR(100),
    Details VARCHAR(max),
    ReferenceId UNIQUEIDENTIFIER NULL,
    ReferenceType int,
    ActivityType int,
    CreatedDate DATETIME2,
    CreatedBy VARCHAR(255),  
    CONSTRAINT ActivityLogs
        PRIMARY KEY (ActivityLogId)
);