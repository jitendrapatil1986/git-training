CREATE TABLE Divisions (
    DivisionId UNIQUEIDENTIFIER NOT NULL,
    DivisionCode VARCHAR(10),
    DivisionName VARCHAR(255),
    CreatedDate DATETIME2,
    CreatedBy VARCHAR(255),
    UpdatedDate DATETIME2,
    UpdatedBy VARCHAR(255)
);

ALTER TABLE Divisions ADD CONSTRAINT DF_Divisions_Id
    DEFAULT NEWSEQUENTIALID() FOR DivisionId;

ALTER TABLE Divisions ADD CONSTRAINT PK_Divisions
    PRIMARY KEY (DivisionId);
