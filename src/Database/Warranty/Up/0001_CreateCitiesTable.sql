CREATE TABLE Cities (
    CityId INT NOT NULL,
    CityCode VARCHAR(3),
    CityName VARCHAR(255),
    CreatedDate DATETIME2,
    CreatedBy VARCHAR(255),
    UpdatedDate DATETIME2,
    UpdatedBy VARCHAR(255)
);

ALTER TABLE Cities ADD CONSTRAINT PK_Cities
    PRIMARY KEY (CityId);
