ALTER TABLE Payments ADD CommunityNumber VARCHAR(8);
ALTER TABLE Payments ALTER COLUMN JobNumber VARCHAR(8);
ALTER TABLE Payments ADD InvoiceNumber VARCHAR(30);
ALTER TABLE Payments ADD Comments VARCHAR(400);
ALTER TABLE Payments DROP COLUMN RequestedDate;

ALTER TABLE imports.PaymentStage ADD CommunityNumber VARCHAR(8);
ALTER TABLE imports.PaymentStage ALTER COLUMN JobNumber VARCHAR(8);
ALTER TABLE imports.PaymentStage ADD InvoiceNumber VARCHAR(30);
ALTER TABLE imports.PaymentStage ADD Comments VARCHAR(400);
ALTER TABLE imports.PaymentStage DROP COLUMN RequestedDate;