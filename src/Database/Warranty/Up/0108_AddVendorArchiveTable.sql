DROP TABLE imports.ArchivedVendorsStage;

CREATE TABLE imports.ArchivedVendorsStage (
    ImportId INT PRIMARY KEY IDENTITY(-2000000000,1),
    JobNumber VARCHAR(4000),
    VendorName VARCHAR(4000),
    VendorEmail VARCHAR(4000),
    VendorNumber VARCHAR(4000), 
    PhoneType VARCHAR(4000),
    PhoneNumber VARCHAR(4000),
    CostCodeDescription VARCHAR(4000),
    CostCode VARCHAR(4000)
)
