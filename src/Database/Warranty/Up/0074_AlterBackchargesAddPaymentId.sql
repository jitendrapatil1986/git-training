ALTER TABLE [Backcharges]
ADD [PaymentId] [uniqueidentifier] NULL
GO

ALTER TABLE [Backcharges]  WITH CHECK ADD CONSTRAINT [FK_Backcharges_PaymentId] FOREIGN KEY([PaymentId])
REFERENCES [dbo].[Payments] ([PaymentId])
GO

ALTER TABLE [Backcharges] CHECK CONSTRAINT [FK_Backcharges_PaymentId]
GO

