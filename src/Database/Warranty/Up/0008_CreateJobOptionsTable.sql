CREATE TABLE JobOptions (
    JobOptionId INT NOT NULL,
    JobId INT,
    OptionNumber VARCHAR(8),
    OptionDescription VARCHAR(500),
    Quantity INT,
    CreatedDate DATETIME2,
    CreatedBy VARCHAR(255),
    UpdatedDate DATETIME2,
    UpdatedBy VARCHAR(255)
);

ALTER TABLE JobOptions ADD CONSTRAINT PK_JobOptions
    PRIMARY KEY (JobOptionId);

ALTER TABLE JobOptions ADD CONSTRAINT FK_JobOptions
    FOREIGN KEY (JobId) REFERENCES Jobs(JobId);
