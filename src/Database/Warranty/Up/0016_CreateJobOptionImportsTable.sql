CREATE TABLE imports.JobOptionImports (
    ImportId INT IDENTITY(1,1),
    JobNumber VARCHAR(4000),
    Quantity VARCHAR(4000),
    OptionNumber VARCHAR(4000),
    OptionDescription VARCHAR(4000)
);

ALTER TABLE imports.JobOptionImports ADD CONSTRAINT PK_JobOptionImports
    PRIMARY KEY (ImportId);