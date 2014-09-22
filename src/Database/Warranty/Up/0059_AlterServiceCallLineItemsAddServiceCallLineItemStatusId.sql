ALTER TABLE [dbo].[ServiceCallLineItems]
ADD [ServiceCallLineItemStatusId] [tinyint] NULL

ALTER TABLE [dbo].[ServiceCallLineItems]  WITH CHECK ADD  CONSTRAINT [FK_ServiceCallLineItems_ServiceCallLineItemStatusId] FOREIGN KEY([ServiceCallLineItemStatusId])
REFERENCES [lookups].[ServiceCallLineItemStatuses] ([ServiceCallLineItemStatusId])

ALTER TABLE [dbo].[ServiceCallLineItems] CHECK CONSTRAINT [FK_ServiceCallLineItems_ServiceCallLineItemStatusId]

--Set all non-completed lines initally to Open status.
UPDATE [ServiceCallLineItems] SET [ServiceCallLineItemStatusId] = 1 WHERE [Completed] <> 1

--Copy Completed bit value to new Status field.
UPDATE [ServiceCallLineItems] SET [ServiceCallLineItemStatusId] = 3 WHERE [Completed] = 1

--Drop Completed field b/c we store the value in the new status field.
ALTER TABLE [dbo].[ServiceCallLineItems] DROP COLUMN [Completed]
