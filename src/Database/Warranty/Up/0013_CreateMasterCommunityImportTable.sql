CREATE TABLE imports.MasterCommunityImports (
        ImportId INT IDENTITY(1,1),
        City VARCHAR(4000), 
        Division VARCHAR(4000), 
        Project VARCHAR(4000), 
        Community VARCHAR(4000), 
        WarrantyServiceRepresentative VARCHAR(4000), 
        AreaPresident VARCHAR(4000), 
        AreaPresidentCode VARCHAR(4000), 
        BuildOnYourLot VARCHAR(4000), 
        CityCode VARCHAR(4000), 
        CommunityId VARCHAR(4000), 
        CommunityNumber VARCHAR(4000), 
        CommunityNumberOld VARCHAR(4000), 
        DivisionCode VARCHAR(4000), 
        DivisionPresidentEmployeeID VARCHAR(4000), 
        DocumentAuthor VARCHAR(4000), 
        DocumentAuthorEmployeeId VARCHAR(4000), 
        DocumentAuthorFax VARCHAR(4000), 
        PlanTypeDesc VARCHAR(4000), 
        ProjectManagerEmployeeID  VARCHAR(4000), 
        ProjectCode VARCHAR(4000), 
        Satelite VARCHAR(4000), 
        SateliteCode VARCHAR(4000), 
        CommunityStatus VARCHAR(4000), 
        StatusDescription VARCHAR(4000), 
        TypeCC VARCHAR(4000)
);

ALTER TABLE imports.MasterCommunityImports ADD CONSTRAINT PK_MasterCommunityImports
    PRIMARY KEY(ImportId);