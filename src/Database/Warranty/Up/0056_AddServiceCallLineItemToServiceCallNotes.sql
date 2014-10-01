ALTER TABLE [ServiceCallNotes]
ADD [ServiceCallLineItemId] [uniqueidentifier] NULL
GO

ALTER TABLE [ServiceCallNotes]  WITH CHECK ADD  CONSTRAINT [FK_ServiceCallNotes_ServiceCallLineItemId] FOREIGN KEY([ServiceCallLineItemId])
REFERENCES [dbo].[ServiceCallLineItems] ([ServiceCallLineItemId])
GO

ALTER TABLE [dbo].[ServiceCallNotes] CHECK CONSTRAINT [FK_ServiceCallNotes_ServiceCallLineItemId]
GO