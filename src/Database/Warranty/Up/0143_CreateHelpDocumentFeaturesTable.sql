if dbo.TableExists('HelpDocumentFeatures') = 0
begin
	CREATE TABLE [dbo].[HelpDocumentFeatures](
		[DocumentFeatureId] [int] IDENTITY(1,1) NOT NULL,
		[DocumentFeatureName] [varchar](100) NOT NULL,
	 CONSTRAINT [PK_HelpDocumentFeatures] PRIMARY KEY CLUSTERED 
	(
		[DocumentFeatureId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	insert into HelpDocumentFeatures (DocumentFeatureName) values ('Administration')
	insert into HelpDocumentFeatures (DocumentFeatureName) values ('Job Page')
	insert into HelpDocumentFeatures (DocumentFeatureName) values ('Line Item')
	insert into HelpDocumentFeatures (DocumentFeatureName) values ('Notifications')
	insert into HelpDocumentFeatures (DocumentFeatureName) values ('Payments')
	insert into HelpDocumentFeatures (DocumentFeatureName) values ('PO''s')
	insert into HelpDocumentFeatures (DocumentFeatureName) values ('Service Call')
	insert into HelpDocumentFeatures (DocumentFeatureName) values ('Reports')
end															 