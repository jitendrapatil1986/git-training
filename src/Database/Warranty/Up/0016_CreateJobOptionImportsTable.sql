CREATE TABLE imports.JobOptionImports (
    ImportId UNIQUEIDENTIFIER NOT NULL,
    JobNumber VARCHAR(4000),
    Quantity VARCHAR(4000),
    OptionNumber VARCHAR(4000),
    OptionDescription VARCHAR(4000)
);

ALTER TABLE imports.JobOptionImports ADD CONSTRAINT DF_JobOptionImports
    DEFAULT NEWSEQUENTIALID() FOR ImportId;

ALTER TABLE imports.JobOptionImports ADD CONSTRAINT PK_JobOptionImports
    PRIMARY KEY (ImportId);