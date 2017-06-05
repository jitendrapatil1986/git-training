if dbo.TableExists('HelpDocumentFeatureItems') = 0
begin
	CREATE TABLE [dbo].[HelpDocumentFeatureItems](
		[DocumentFeatureItemId] [int] IDENTITY(1,1) NOT NULL,
		[DocumentFeatureId] [int] NULL,
		[DocumentFeatureItemName] [varchar](50) NOT NULL,
	 CONSTRAINT [PK_HelpDocumentFeatureItems] PRIMARY KEY CLUSTERED 
	(
		[DocumentFeatureItemId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[HelpDocumentFeatureItems]  WITH CHECK ADD  CONSTRAINT [FK_HelpDocumentFeatureItems_HelpDocumentFeatures] FOREIGN KEY([DocumentFeatureId])
	REFERENCES [dbo].[HelpDocumentFeatures] ([DocumentFeatureId])

	ALTER TABLE [dbo].[HelpDocumentFeatureItems] CHECK CONSTRAINT [FK_HelpDocumentFeatureItems_HelpDocumentFeatures]
	
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName) values (1,'Add and Edit Contact Information')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName) values (1,'Creating Address Labels ')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName) values (1,'My Team Tab')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName) values (1,'Reassign a Community')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName) values (2,'Vendors')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName) values (3,'Add & Edit Line Items ')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName) values (3,'Add Attachments')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName) values (3,'Add Notes')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName) values (3,'Completing a Line Item')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName) values (4,'To Do Notifications')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName) values (5,'Payment and Backcharge Request')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName) values (6,'Requesting POs')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName) values (7,'Create Call')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName) values (7,'Escalate a Call')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName) values (7,'Reassign a Call')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName) values (7,'Closing a Service Call')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName) values (7,'No Action')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName) values (7,'Special Project Calls')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName) values (8,'Warranty $ Spent')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName) values (8,'Warranty Achievement Report')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName) values (8,'WSR Call Summary Report')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName) values (8,'WSR Loading Report')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName) values (8,'WSR Open Closed Call Report')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName) values (8,'WSR Summary Report')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName) values (8,'Saltine Report')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName) values (8,'Mail Merge Report')
End
