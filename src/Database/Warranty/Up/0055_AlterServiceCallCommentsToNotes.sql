--rename PK constraint.
EXEC sp_rename 'dbo.PK_ServiceCallComments', 'PK_ServiceCallNotes'

--recreate FK constraint b/c rename causes issues.
ALTER TABLE [dbo].[ServiceCallComments] DROP CONSTRAINT [FK_ServiceCallComments_ServiceCallId]

ALTER TABLE [dbo].[ServiceCallComments]  WITH CHECK ADD  CONSTRAINT [FK_ServiceCallNotes_ServiceCallId] FOREIGN KEY([ServiceCallId])
REFERENCES [dbo].[ServiceCalls] ([ServiceCallId])
GO

ALTER TABLE [dbo].[ServiceCallComments] CHECK CONSTRAINT [FK_ServiceCallNotes_ServiceCallId]
GO

--recreate Default constraint b/c rename causes issues.
ALTER TABLE [dbo].[ServiceCallComments] DROP CONSTRAINT [ServiceCallComments_Id]
ALTER TABLE [dbo].[ServiceCallComments] ADD  CONSTRAINT [ServiceCallNotes_Id]  DEFAULT (newsequentialid()) FOR [ServiceCallCommentId]

--rename columns
EXEC sp_rename 'dbo.ServiceCallComments.ServiceCallCommentId', 'ServiceCallNoteId', 'COLUMN'
EXEC sp_rename 'dbo.ServiceCallComments.ServiceCallComment', 'ServiceCallNote', 'COLUMN'

--rename table.
EXEC sp_rename ServiceCallComments, ServiceCallNotes