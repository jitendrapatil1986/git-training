ALTER TABLE Payments DROP COLUMN Comments;
ALTER TABLE imports.PaymentStage DROP COLUMN Comments;

GO

ALTER TABLE Payments ADD HoldComments VARCHAR(400) NULL;
ALTER TABLE Payments ADD VarianceReason VARCHAR(400) NULL;
ALTER TABLE imports.PaymentStage ADD HoldComments VARCHAR(400) NULL;
ALTER TABLE imports.PaymentStage ADD VarianceReason VARCHAR(400) NULL;