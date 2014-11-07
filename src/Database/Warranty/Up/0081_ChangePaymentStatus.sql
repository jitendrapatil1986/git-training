ALTER TABLE Payments
DROP COLUMN PaymentStatus
GO

ALTER TABLE Payments
ADD PaymentStatus int 
GO