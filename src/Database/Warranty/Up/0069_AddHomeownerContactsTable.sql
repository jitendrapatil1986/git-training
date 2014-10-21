CREATE TABLE [dbo].[HomeownerContacts](
	[HomeownerContactId] [uniqueidentifier] NOT NULL,
	[HomeownerId] [uniqueidentifier] NOT NULL,
	[ContactType] [tinyint] NOT NULL,
	[ContactValue] [varchar](255) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [varchar](255) NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [varchar](255) NULL,
 CONSTRAINT [PK_HomeownerContacts] PRIMARY KEY CLUSTERED 
(
	[HomeownerContactId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [ActiveData]
) ON [ActiveData]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[HomeownerContacts] ADD  CONSTRAINT [DF_HomeownerContacts_HomeownerContactId]  DEFAULT (newsequentialid()) FOR [HomeownerContactId]
GO

ALTER TABLE [dbo].[HomeownerContacts]  WITH CHECK ADD  CONSTRAINT [FK_HomeownerContacts_HomeownerId] FOREIGN KEY([HomeownerId])
REFERENCES [dbo].[Homeowners] ([HomeownerId])
GO

ALTER TABLE [dbo].[HomeownerContacts] CHECK CONSTRAINT [FK_HomeownerContacts_HomeownerId]
GO


