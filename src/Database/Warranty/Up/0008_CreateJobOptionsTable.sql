CREATE TABLE JobOptions (
    JobOptionId UNIQUEIDENTIFIER NOT NULL,
    JobId UNIQUEIDENTIFIER,
    OptionNumber VARCHAR(8),
    OptionDescription VARCHAR(500),
    Quantity INT,
    CreatedDate DATETIME2,
    CreatedBy VARCHAR(255),
    UpdatedDate DATETIME2,
    UpdatedBy VARCHAR(255)
);

ALTER TABLE JobOptions ADD CONSTRAINT DF_JobOptions_Id
    DEFAULT NEWSEQUENTIALID() FOR JobOptionId;

ALTER TABLE JobOptions ADD CONSTRAINT PK_JobOptions
    PRIMARY KEY (JobOptionId);

ALTER TABLE JobOptions ADD CONSTRAINT FK_JobOptions
    FOREIGN KEY (JobId) REFERENCES Jobs(JobId);
