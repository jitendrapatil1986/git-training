CREATE TABLE Payments(
    PaymentId UNIQUEIDENTIFIER NOT NULL CONSTRAINT
        DF_Payments_PaymentId DEFAULT NEWSEQUENTIALID(),
    VendorNumber VARCHAR(8) NULL,    
    Amount DECIMAL(18, 2) NULL,
    PaymentStatus VARCHAR(10) NULL,        
    JobNumber VARCHAR(8) NULL,            
    JdeIdentifier VARCHAR(50) NULL,        
    CONSTRAINT PK_Payments 
        PRIMARY KEY (PaymentId)
);