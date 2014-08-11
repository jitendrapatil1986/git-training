CREATE TABLE JobOptions (
    JobOptionId UNIQUEIDENTIFIER NOT NULL CONSTRAINT
        DF_JobOptions_Id DEFAULT NEWSEQUENTIALID(),
    JobId UNIQUEIDENTIFIER,
    OptionNumber VARCHAR(8),
    OptionDescription VARCHAR(500),
    Quantity INT,
    CreatedDate DATETIME2,
    CreatedBy VARCHAR(255),
    UpdatedDate DATETIME2,
    UpdatedBy VARCHAR(255),
    CONSTRAINT PK_JobOptions
        PRIMARY KEY (JobOptionId),
    CONSTRAINT FK_JobOptions
        FOREIGN KEY (JobId) REFERENCES Jobs(JobId)
);