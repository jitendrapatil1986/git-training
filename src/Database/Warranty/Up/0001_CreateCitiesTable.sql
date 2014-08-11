CREATE TABLE Cities (
    CityId UNIQUEIDENTIFIER NOT NULL CONSTRAINT 
        DF_Cities_Id DEFAULT NEWSEQUENTIALID(),
    CityCode VARCHAR(10),
    CityName VARCHAR(255),
    CreatedDate DATETIME2,
    CreatedBy VARCHAR(255),
    UpdatedDate DATETIME2,
    UpdatedBy VARCHAR(255),
    CONSTRAINT PK_Cities
        PRIMARY KEY (CityId)
);
