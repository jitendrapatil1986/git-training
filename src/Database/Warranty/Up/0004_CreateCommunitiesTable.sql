CREATE TABLE Communities (
    CommunityId UNIQUEIDENTIFIER NOT NULL,
    CommunityNumber VARCHAR(8),
    CommunityName VARCHAR(255),
    CityId UNIQUEIDENTIFIER,
    DivisionId UNIQUEIDENTIFIER,
    ProjectId UNIQUEIDENTIFIER,
    SateliteCityId UNIQUEIDENTIFIER,
    CommunityStatusCode VARCHAR(10),    
    CommunityStatusDescription VARCHAR(30),
    ProductType VARCHAR(20),
    ProductTypeDescription VARCHAR(30),
    CreatedDate DATETIME2,
    CreatedBy VARCHAR(255),
    UpdatedDate DATETIME2,
    UpdatedBy VARCHAR(255)
);

ALTER TABLE Communities ADD CONSTRAINT PK_Communities
    PRIMARY KEY (CommunityId);

ALTER TABLE Communities ADD CONSTRAINT FK_Communities_CityCode
    FOREIGN KEY (CityId) REFERENCES Cities(CityId);

ALTER TABLE Communities ADD CONSTRAINT FK_Communities_DivisionCode
    FOREIGN KEY (DivisionId) REFERENCES Divisions(DivisionId);

ALTER TABLE Communities ADD CONSTRAINT FK_Communities_ProjectNumber
    FOREIGN KEY (ProjectId) REFERENCES Projects(ProjectId);

ALTER TABLE Communities ADD CONSTRAINT FK_Communities_SateliteCityCode
    FOREIGN KEY (SateliteCityId) REFERENCES Cities(CityId);