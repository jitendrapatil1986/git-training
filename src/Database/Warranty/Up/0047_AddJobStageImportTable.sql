CREATE TABLE imports.JobStageImports (
    JobStageImportId INT IDENTITY(1,1),
    JobNumber VARCHAR(8),
    JobStage INT,
    CompletionDate DATE,
    JdeIdentifier VARCHAR(25),
    CONSTRAINT PK_JobStageImports
        PRIMARY KEY (JobStageImportId)
)