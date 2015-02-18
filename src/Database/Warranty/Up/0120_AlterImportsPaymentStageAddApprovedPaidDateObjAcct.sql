ALTER TABLE imports.[PaymentStage]
ADD ObjectAccount varchar(30)  NULL
GO

ALTER TABLE imports.[PaymentStage]
ADD ApprovedDate datetime2  NULL
GO

ALTER TABLE imports.[PaymentStage]
ADD PaidDate datetime2  NULL
GO