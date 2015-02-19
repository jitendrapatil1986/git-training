ALTER TABLE [Backcharges]
ADD JobNumber varchar(8) NULL
GO

ALTER TABLE [Backcharges]
ADD ObjectAccount varchar(255) NULL
GO

ALTER TABLE [Backcharges]
ADD ServiceCallLineItemId uniqueidentifier NULL
GO

ALTER TABLE [dbo].[Backcharges]  WITH CHECK ADD  CONSTRAINT [FK_Backcharges_ServiceCallLineItemId] FOREIGN KEY([ServiceCallLineItemId])
REFERENCES [dbo].[ServiceCallLineItems] ([ServiceCallLineItemId])
GO

ALTER TABLE [dbo].[Backcharges] CHECK CONSTRAINT [FK_Backcharges_ServiceCallLineItemId]
GO
