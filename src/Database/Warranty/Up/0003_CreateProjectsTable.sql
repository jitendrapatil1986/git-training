CREATE TABLE Projects (
    ProjectId UNIQUEIDENTIFIER NOT NULL,
    ProjectNumber VARCHAR(3),
    ProjectName VARCHAR(255),
    CreatedDate DATETIME2,
    CreatedBy VARCHAR(255),
    UpdatedDate DATETIME2,
    UpdatedBy VARCHAR(255)
);

ALTER TABLE Projects ADD CONSTRAINT DF_Projects_Id
    DEFAULT NEWSEQUENTIALID() FOR ProjectId;

ALTER TABLE Projects ADD CONSTRAINT PK_Projects
    PRIMARY KEY (ProjectId);
