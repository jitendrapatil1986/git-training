CREATE TABLE imports.WarrantyPaymentStage (
  WarrantyPaymentStageId INT NOT NULL IDENTITY(-20000000,1)
, PostingMonth INT
, PostingYear INT
, CostCenter VARCHAR(8)
, ObjectAccount VARCHAR(8)
, CostCode VARCHAR(8)
, CostCodeDescription VARCHAR(250)
, JobNumber VARCHAR(8)
, InvoiceNumber VARCHAR(30)
, DatePosted DATETIME2
, Amount DECIMAL(18, 2)
, VendorNumber VARCHAR(8)
, ExplanationName VARCHAR(250)
, ExplanationRemark VARCHAR(250)
, JdeIdentifier VARCHAR(250)
, CONSTRAINT PK_WarrantyPaymentStage
    PRIMARY KEY (WarrantyPaymentStageId)
)