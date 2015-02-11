IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE NAME = 'IDX_Payments_PaymentStatus')
BEGIN
CREATE NONCLUSTERED INDEX IDX_Payments_PaymentStatus
ON [dbo].[Payments] ([PaymentStatus])
INCLUDE ([PaymentId],[ServiceCallLineItemId])
END