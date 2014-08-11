CREATE TABLE imports.JobOptionImports (
    ImportId UNIQUEIDENTIFIER NOT NULL CONSTRAINT 
        DF_JobOptionImports DEFAULT NEWSEQUENTIALID(),
    JobNumber VARCHAR(4000),
    Quantity VARCHAR(4000),
    OptionNumber VARCHAR(4000),
    OptionDescription VARCHAR(4000),
    CONSTRAINT PK_JobOptionImports
        PRIMARY KEY (ImportId)
);