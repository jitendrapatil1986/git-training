CREATE TABLE [dbo].[JobAttachments](
	[JobAttachmentId] [uniqueidentifier] NOT NULL,
	[JobId] [uniqueidentifier] NOT NULL,
	[FilePath] [varchar](max) NULL,
	[DisplayName] [varchar](255) NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [varchar](255) NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [varchar](255) NULL,
 CONSTRAINT [PK_JobAttachments] PRIMARY KEY CLUSTERED 
(
	[JobAttachmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [ActiveData]
) ON [ActiveData] TEXTIMAGE_ON [ActiveData]

GO

ALTER TABLE [dbo].[JobAttachments] ADD  CONSTRAINT [JobAttachments_Id]  DEFAULT (newsequentialid()) FOR [JobAttachmentId]
GO

ALTER TABLE [dbo].[JobAttachments]  WITH CHECK ADD  CONSTRAINT [FK_JobAttachments_JobId] FOREIGN KEY([JobId])
REFERENCES [dbo].[Jobs] ([JobId])
GO

ALTER TABLE [dbo].[JobAttachments] CHECK CONSTRAINT [FK_JobAttachments_JobId]
GO
