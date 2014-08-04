CREATE TABLE Divisions (
    DivisionId INT NOT NULL,
    DivisionCode VARCHAR(3),
    DivisionName VARCHAR(255),
    CreatedDate DATETIME2,
    CreatedBy VARCHAR(255),
    UpdatedDate DATETIME2,
    UpdatedBy VARCHAR(255)
);

ALTER TABLE Divisions ADD CONSTRAINT PK_Divisions
    PRIMARY KEY (DivisionId);
