CREATE TABLE Projects (
    ProjectId UNIQUEIDENTIFIER NOT NULL CONSTRAINT
        DF_Projects_Id DEFAULT NEWSEQUENTIALID(),
    ProjectNumber VARCHAR(3),
    ProjectName VARCHAR(255),
    CreatedDate DATETIME2,
    CreatedBy VARCHAR(255),
    UpdatedDate DATETIME2,
    UpdatedBy VARCHAR(255),
    CONSTRAINT PK_Projects
        PRIMARY KEY (ProjectId)
);