CREATE TABLE JobStages (
    JobId UNIQUEIDENTIFIER,
    JobStage INT,
    CompletionDate DATE,
    JdeIdentifier VARCHAR(25),
	CreatedDate datetime2(7) NULL,
	CreatedBy varchar(255) NULL,
	UpdatedDate datetime2(7) NULL,
	UpdatedBy varchar(255) NULL,
    CONSTRAINT PK_JobStages
        PRIMARY KEY (JobId, JobStage)
)