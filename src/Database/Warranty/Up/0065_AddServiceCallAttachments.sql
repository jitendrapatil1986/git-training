CREATE TABLE [dbo].[ServiceCallAttachments](
	[ServiceCallAttachmentId] [uniqueidentifier] NOT NULL,
	[ServiceCallId] [uniqueidentifier] NOT NULL,
	[FilePath] [varchar](max) NULL,
	[DisplayName] [varchar](255) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [varchar](255) NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [varchar](255) NULL,
 CONSTRAINT [PK_ServiceCallAttachments] PRIMARY KEY CLUSTERED 
(
	[ServiceCallAttachmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [ActiveData]
) ON [ActiveData] TEXTIMAGE_ON [ActiveData]

GO

ALTER TABLE [dbo].[ServiceCallAttachments] ADD  CONSTRAINT [ServiceCallAttachments_Id]  DEFAULT (newsequentialid()) FOR [ServiceCallAttachmentId]
GO

ALTER TABLE [dbo].[ServiceCallAttachments]  WITH CHECK ADD  CONSTRAINT [FK_ServiceCallAttachments_ServiceCallId] FOREIGN KEY([ServiceCallId])
REFERENCES [dbo].[ServiceCalls] ([ServiceCallId])
GO

ALTER TABLE [dbo].[ServiceCallAttachments] CHECK CONSTRAINT [FK_ServiceCallAttachments_ServiceCallId]
GO


