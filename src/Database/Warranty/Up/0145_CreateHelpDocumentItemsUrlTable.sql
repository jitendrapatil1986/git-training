if dbo.TableExists('HelpDocumentItemsUrls') = 0
begin
	CREATE TABLE [dbo].[HelpDocumentItemsUrls](
		[UrlId] [int] IDENTITY(1,1) NOT NULL,
		[DocumentFeatureItemId] [int] NULL,
		[Url] [varchar](max) NULL,
		[ItemDocumentName] [varchar](100) NULL,
	 CONSTRAINT [PK_HelpDocumentItemsUrl] PRIMARY KEY CLUSTERED 
	(
		[UrlId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

	ALTER TABLE [dbo].[HelpDocumentItemsUrls]  WITH CHECK ADD  CONSTRAINT [FK_HelpDocumentItemsUrls_HelpDocumentFeatureItems] FOREIGN KEY([DocumentFeatureItemId])
	REFERENCES [dbo].[HelpDocumentFeatureItems] ([DocumentFeatureItemId])

	ALTER TABLE [dbo].[HelpDocumentItemsUrls] CHECK CONSTRAINT [FK_HelpDocumentItemsUrls_HelpDocumentFeatureItems]

	insert into HelpDocumentItemsUrls (DocumentFeatureItemId,Url,ItemDocumentName) values (1,'https://documentation.davidweekleyhomes.com/DWHelp/Production/1249_819/Add%20and%20Edit%20Contact%20Information-BPP.pdf','PDF')
	insert into HelpDocumentItemsUrls (DocumentFeatureItemId,Url,ItemDocumentName) values (2,'https://documentation.davidweekleyhomes.com/DWHelp/Production/4154_954/Creating%20Address%20Labels-BPP.pdf','PDF')
	insert into HelpDocumentItemsUrls (DocumentFeatureItemId,Url,ItemDocumentName) values (3,'https://documentation.davidweekleyhomes.com/DWHelp/Production/1234_58/My%20Team%20-%20Closed%20Service%20Requests-BPP.pdf','PDF')
	insert into HelpDocumentItemsUrls (DocumentFeatureItemId,Url,ItemDocumentName) values (4,'https://documentation.davidweekleyhomes.com/DWHelp/Production/4931_861/Reassign%20a%20Community-BPP.pdf','PDF')
	insert into HelpDocumentItemsUrls (DocumentFeatureItemId,Url,ItemDocumentName) values (5,'https://documentation.davidweekleyhomes.com/DWHelp/Production/1253_488/Job%20Page%20-%20Vendors-BPP.pdf','PDF')
	insert into HelpDocumentItemsUrls (DocumentFeatureItemId,Url,ItemDocumentName) values (6,'https://documentation.davidweekleyhomes.com/DWHelp/Production/1294_471/Service%20Call%20Page%20-%20Add%20and%20Edit%20Line%20Item-BPP.pdf','PDF')
	insert into HelpDocumentItemsUrls (DocumentFeatureItemId,Url,ItemDocumentName) values (7,'https://documentation.davidweekleyhomes.com/DWHelp/Production/1257_515/Add%20Attachments-BPP.pdf','PDF')
	insert into HelpDocumentItemsUrls (DocumentFeatureItemId,Url,ItemDocumentName) values (8,'https://documentation.davidweekleyhomes.com/DWHelp/Production/1261_447/Add%20Notes-BPP.pdf','PDF')
	insert into HelpDocumentItemsUrls (DocumentFeatureItemId,Url,ItemDocumentName) values (9,'https://documentation.davidweekleyhomes.com/DWHelp/Production/4370_190/Completing%20a%20Line%20Item-BPP.pdf','PDF')
	insert into HelpDocumentItemsUrls (DocumentFeatureItemId,Url,ItemDocumentName) values (10,'https://documentation.davidweekleyhomes.com/DWHelp/Production/2946_212/Job%20Changed%20Task%20Notifications-BPP.pdf','PDF')
	insert into HelpDocumentItemsUrls (DocumentFeatureItemId,Url,ItemDocumentName) values (11,'https://documentation.davidweekleyhomes.com/DWHelp/Production/2435_868/Payment%20and%20Backcharge%20Request-BPP.pdf','PDF')
	insert into HelpDocumentItemsUrls (DocumentFeatureItemId,Url,ItemDocumentName) values (12,'https://documentation.davidweekleyhomes.com/DWHelp/Production/4931_861/Reassign%20a%20Community-BPP.pdf','PDF')
	insert into HelpDocumentItemsUrls (DocumentFeatureItemId,Url,ItemDocumentName) values (13,'https://documentation.davidweekleyhomes.com/DWHelp/Production/2422_516/Create%20Call-BPP.pdf','PDF')
	insert into HelpDocumentItemsUrls (DocumentFeatureItemId,Url,ItemDocumentName) values (14,'https://documentation.davidweekleyhomes.com/DWHelp/Production/1282_316/Service%20Call%20Page%20-%20Escalate%20Request-BPP.pdf','PDF')
	insert into HelpDocumentItemsUrls (DocumentFeatureItemId,Url,ItemDocumentName) values (15,'https://documentation.davidweekleyhomes.com/DWHelp/Production/4899_839/Reassign%20a%20Call-BPP.pdf','PDF')
	insert into HelpDocumentItemsUrls (DocumentFeatureItemId,Url,ItemDocumentName) values (16,'https://documentation.davidweekleyhomes.com/DWHelp/Production/4383_266/Closing%20a%20Service%20Call-BPP.pdf','PDF')
	insert into HelpDocumentItemsUrls (DocumentFeatureItemId,Url,ItemDocumentName) values (17,'https://documentation.davidweekleyhomes.com/DWHelp/Production/2374_395/Line%20Item%20Page%20-%20No%20Action-BPP.pdf','PDF')
	insert into HelpDocumentItemsUrls (DocumentFeatureItemId,Url,ItemDocumentName) values (18,'https://documentation.davidweekleyhomes.com/DWHelp/Production/1269_556/Service%20Call%20Page%20-%20Special%20Project-BPP.pdf','PDF')
	insert into HelpDocumentItemsUrls (DocumentFeatureItemId,Url,ItemDocumentName) values (19,'https://documentation.davidweekleyhomes.com/DWHelp/Production/2898_785/Dashboard%20-%20Metrics%20(Warranty%20$%20Spent)-BPP.pdf','PDF')
	insert into HelpDocumentItemsUrls (DocumentFeatureItemId,Url,ItemDocumentName) values (20,'https://documentation.davidweekleyhomes.com/DWHelp/Production/1218_483/Reports%20-%20Warranty%20Achievement%20Report-BPP.pdf','PDF')
	insert into HelpDocumentItemsUrls (DocumentFeatureItemId,Url,ItemDocumentName) values (21,'https://documentation.davidweekleyhomes.com/DWHelp/Production/4533_972/WSR%20Call%20Summary%20Report-BPP.pdf','PDF')
	insert into HelpDocumentItemsUrls (DocumentFeatureItemId,Url,ItemDocumentName) values (22,'https://documentation.davidweekleyhomes.com/DWHelp/Production/4504_942/WSR%20Loading%20Report-BPP.pdf','PDF')
	insert into HelpDocumentItemsUrls (DocumentFeatureItemId,Url,ItemDocumentName) values (23,'https://documentation.davidweekleyhomes.com/DWHelp/Production/4984_856/WSR%20Open%20Closed%20Call%20Report-BPP.pdf','PDF')
	insert into HelpDocumentItemsUrls (DocumentFeatureItemId,Url,ItemDocumentName) values (24,'https://documentation.davidweekleyhomes.com/DWHelp/Production/4549_578/WSR%20Summary%20Report-BPP.pdf','PDF')
	insert into HelpDocumentItemsUrls (DocumentFeatureItemId,Url,ItemDocumentName) values (25,'https://documentation.davidweekleyhomes.com/DWHelp/Production/4972_151/Saltline%20Report-BPP.pdf','PDF')
	insert into HelpDocumentItemsUrls (DocumentFeatureItemId,Url,ItemDocumentName) values (26,'https://documentation.davidweekleyhomes.com/DWHelp/Production/1206_675/Reports%20-%20Mail%20Merge%20Report-BPP.pdf','PDF')
End
