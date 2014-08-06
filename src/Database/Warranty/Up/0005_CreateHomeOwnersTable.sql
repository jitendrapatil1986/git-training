CREATE TABLE HomeOwners (
    HomeOwnerId UNIQUEIDENTIFIER NOT NULL,
    JobId UNIQUEIDENTIFIER,
    HomeOwnerNumber INT,
    HomeOwnerName VARCHAR(255),
    HomePhone VARCHAR(255),
    OtherPhone VARCHAR(255),
    WorkPhone1 VARCHAR(255),
    WorkPhone2 VARCHAR(255),
    EmailAddress VARCHAR(255),
    CreatedDate DATETIME2,
    CreatedBy VARCHAR(255),
    UpdatedDate DATETIME2,
    UpdatedBy VARCHAR(255)
);

ALTER TABLE HomeOwners ADD CONSTRAINT PK_HomeOwners
    PRIMARY KEY (HomeOwnerId);
