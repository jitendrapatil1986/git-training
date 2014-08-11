CREATE TABLE Divisions (
    DivisionId UNIQUEIDENTIFIER NOT NULL CONSTRAINT 
        DF_Divisions_Id DEFAULT NEWSEQUENTIALID(),
    DivisionCode VARCHAR(10),
    DivisionName VARCHAR(255),
    CreatedDate DATETIME2,
    CreatedBy VARCHAR(255),
    UpdatedDate DATETIME2,
    UpdatedBy VARCHAR(255),
    CONSTRAINT PK_Divisions
        PRIMARY KEY (DivisionId)
);