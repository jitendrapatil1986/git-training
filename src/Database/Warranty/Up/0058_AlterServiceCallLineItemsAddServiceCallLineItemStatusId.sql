ALTER TABLE [dbo].[ServiceCallLineItems]
ADD [ServiceCallLineItemStatusId] [tinyint] NULL

GO

--Set all non-completed lines initally to Open status.
UPDATE [ServiceCallLineItems] SET [ServiceCallLineItemStatusId] = 1 WHERE [Completed] <> 1

--Copy Completed bit value to new Status field.
UPDATE [ServiceCallLineItems] SET [ServiceCallLineItemStatusId] = 3 WHERE [Completed] = 1

--Drop Completed field b/c we store the value in the new status field.
ALTER TABLE [dbo].[ServiceCallLineItems] DROP COLUMN [Completed]
