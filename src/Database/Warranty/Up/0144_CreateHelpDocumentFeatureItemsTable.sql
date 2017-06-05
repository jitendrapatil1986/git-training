if dbo.TableExists('HelpDocumentFeatureItems') = 0
begin
	CREATE TABLE [dbo].[HelpDocumentFeatureItems](
		[DocumentFeatureItemId] [int] IDENTITY(1,1) NOT NULL,
		[DocumentFeatureId] [int] NULL,
		[DocumentFeatureItemName] [varchar](50) NOT NULL,
		[Url] [varchar](max) NULL,
	 CONSTRAINT [PK_HelpDocumentFeatureItems] PRIMARY KEY CLUSTERED 
	(
		[DocumentFeatureItemId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[HelpDocumentFeatureItems]  WITH CHECK ADD  CONSTRAINT [FK_HelpDocumentFeatureItems_HelpDocumentFeatures] FOREIGN KEY([DocumentFeatureId])
	REFERENCES [dbo].[HelpDocumentFeatures] ([DocumentFeatureId])

	ALTER TABLE [dbo].[HelpDocumentFeatureItems] CHECK CONSTRAINT [FK_HelpDocumentFeatureItems_HelpDocumentFeatures]
	
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName, Url) values (1,'Add and Edit Contact Information','https://documentation.davidweekleyhomes.com/DWHelp/Production/1249_819/Add%20and%20Edit%20Contact%20Information-BPP.pdf')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName, Url) values (1,'Creating Address Labels','https://documentation.davidweekleyhomes.com/DWHelp/Production/4154_954/Creating%20Address%20Labels-BPP.pdf')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName, Url) values (1,'My Team Tab','https://documentation.davidweekleyhomes.com/DWHelp/Production/1234_58/My%20Team%20-%20Closed%20Service%20Requests-BPP.pdf')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName, Url) values (1,'Reassign a Community','https://documentation.davidweekleyhomes.com/DWHelp/Production/4931_861/Reassign%20a%20Community-BPP.pdf')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName, Url) values (2,'Vendors','https://documentation.davidweekleyhomes.com/DWHelp/Production/1253_488/Job%20Page%20-%20Vendors-BPP.pdf')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName, Url) values (3,'Add & Edit Line Items','https://documentation.davidweekleyhomes.com/DWHelp/Production/1294_471/Service%20Call%20Page%20-%20Add%20and%20Edit%20Line%20Item-BPP.pdf')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName, Url) values (3,'Add Attachments','https://documentation.davidweekleyhomes.com/DWHelp/Production/1257_515/Add%20Attachments-BPP.pdf')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName, Url) values (3,'Add Notes','https://documentation.davidweekleyhomes.com/DWHelp/Production/1261_447/Add%20Notes-BPP.pdf')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName, Url) values (3,'Completing a Line Item','https://documentation.davidweekleyhomes.com/DWHelp/Production/4370_190/Completing%20a%20Line%20Item-BPP.pdf')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName, Url) values (4,'To Do Notifications','https://documentation.davidweekleyhomes.com/DWHelp/Production/2946_212/Job%20Changed%20Task%20Notifications-BPP.pdf')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName, Url) values (5,'Payment and Backcharge Request','https://documentation.davidweekleyhomes.com/DWHelp/Production/2435_868/Payment%20and%20Backcharge%20Request-BPP.pdf')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName, Url) values (6,'Requesting POs','https://documentation.davidweekleyhomes.com/DWHelp/Production/4931_861/Reassign%20a%20Community-BPP.pdf')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName, Url) values (7,'Create Call','https://documentation.davidweekleyhomes.com/DWHelp/Production/2422_516/Create%20Call-BPP.pdf')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName, Url) values (7,'Escalate a Call','https://documentation.davidweekleyhomes.com/DWHelp/Production/1282_316/Service%20Call%20Page%20-%20Escalate%20Request-BPP.pdf')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName, Url) values (7,'Reassign a Call','https://documentation.davidweekleyhomes.com/DWHelp/Production/4899_839/Reassign%20a%20Call-BPP.pdf')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName, Url) values (7,'Closing a Service Call','https://documentation.davidweekleyhomes.com/DWHelp/Production/4383_266/Closing%20a%20Service%20Call-BPP.pdf')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName, Url) values (7,'No Action','https://documentation.davidweekleyhomes.com/DWHelp/Production/2374_395/Line%20Item%20Page%20-%20No%20Action-BPP.pdf')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName, Url) values (7,'Special Project Calls','https://documentation.davidweekleyhomes.com/DWHelp/Production/1269_556/Service%20Call%20Page%20-%20Special%20Project-BPP.pdf')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName, Url) values (8,'Warranty $ Spent','https://documentation.davidweekleyhomes.com/DWHelp/Production/2898_785/Dashboard%20-%20Metrics%20(Warranty%20$%20Spent)-BPP.pdf')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName, Url) values (8,'Warranty Achievement Report','https://documentation.davidweekleyhomes.com/DWHelp/Production/1218_483/Reports%20-%20Warranty%20Achievement%20Report-BPP.pdf')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName, Url) values (8,'WSR Call Summary Report','https://documentation.davidweekleyhomes.com/DWHelp/Production/4533_972/WSR%20Call%20Summary%20Report-BPP.pdf')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName, Url) values (8,'WSR Loading Report','https://documentation.davidweekleyhomes.com/DWHelp/Production/4504_942/WSR%20Loading%20Report-BPP.pdf')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName, Url) values (8,'WSR Open Closed Call Report','https://documentation.davidweekleyhomes.com/DWHelp/Production/4984_856/WSR%20Open%20Closed%20Call%20Report-BPP.pdf')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName, Url) values (8,'WSR Summary Report','https://documentation.davidweekleyhomes.com/DWHelp/Production/4549_578/WSR%20Summary%20Report-BPP.pdf')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName, Url) values (8,'Saltine Report','https://documentation.davidweekleyhomes.com/DWHelp/Production/4972_151/Saltline%20Report-BPP.pdf')
	Insert into HelpDocumentFeatureItems (DocumentFeatureId, DocumentFeatureItemName, Url) values (8,'Mail Merge Report','https://documentation.davidweekleyhomes.com/DWHelp/Production/1206_675/Reports%20-%20Mail%20Merge%20Report-BPP.pdf')
End
