CREATE TABLE Communities (
    CommunityId UNIQUEIDENTIFIER NOT NULL CONSTRAINT
        DF_Communities_Id DEFAULT NEWSEQUENTIALID(),
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
    UpdatedBy VARCHAR(255),
    CONSTRAINT PK_Communities
        PRIMARY KEY (CommunityId),
    CONSTRAINT FK_Communities_CityCode
        FOREIGN KEY (CityId) REFERENCES Cities(CityId),
    CONSTRAINT FK_Communities_DivisionCode
        FOREIGN KEY (DivisionId) REFERENCES Divisions(DivisionId),
    CONSTRAINT FK_Communities_ProjectNumber
        FOREIGN KEY (ProjectId) REFERENCES Projects(ProjectId),
    CONSTRAINT FK_Communities_SateliteCityCode
        FOREIGN KEY (SateliteCityId) REFERENCES Cities(CityId)
);