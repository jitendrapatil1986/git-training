if dbo.TableExists('HelpDocumentFeatures') = 0
begin
	CREATE TABLE [dbo].[HelpDocumentFeatures](
		[DocFeatureId] [int] IDENTITY(1,1) NOT NULL,
		[DocFeatureName] [varchar](100) NOT NULL,
	 CONSTRAINT [PK_HelpDocumentFeatures] PRIMARY KEY CLUSTERED 
	(
		[DocFeatureId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	insert into HelpDocumentFeatures (DocFeatureName) values ('Administration')
	insert into HelpDocumentFeatures (DocFeatureName) values ('Job Page')
	insert into HelpDocumentFeatures (DocFeatureName) values ('Line Item')
	insert into HelpDocumentFeatures (DocFeatureName) values ('Notifications')
	insert into HelpDocumentFeatures (DocFeatureName) values ('Payments')
	insert into HelpDocumentFeatures (DocFeatureName) values ('PO’s')
	insert into HelpDocumentFeatures (DocFeatureName) values ('Service Call')
	insert into HelpDocumentFeatures (DocFeatureName) values ('Reports')
end															 