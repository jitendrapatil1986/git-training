ALTER TABLE [Payments]
ADD [ServiceCallLineItemId] [uniqueidentifier] NULL
GO

ALTER TABLE [Payments]  WITH CHECK ADD CONSTRAINT [FK_Payments_ServiceCallLineItemId] FOREIGN KEY([ServiceCallLineItemId])
REFERENCES [dbo].[ServiceCallLineItems] ([ServiceCallLineItemId])
GO

ALTER TABLE [Payments] CHECK CONSTRAINT [FK_Payments_ServiceCallLineItemId]
GO