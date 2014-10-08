CREATE TABLE [dbo].[JobNotes](
	[JobNoteId] [uniqueidentifier] NOT NULL,
	[JobId] [uniqueidentifier] NOT NULL,
	[Note] [varchar](max) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [varchar](255) NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [varchar](255) NULL,
 CONSTRAINT [PK_JobNotes] PRIMARY KEY CLUSTERED 
(
	[JobNoteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [ActiveData]
) ON [ActiveData] TEXTIMAGE_ON [ActiveData]

GO

ALTER TABLE [dbo].[JobNotes] ADD  CONSTRAINT [JobNotes_Id]  DEFAULT (newsequentialid()) FOR [JobNoteId]
GO

ALTER TABLE [dbo].[JobNotes]  WITH CHECK ADD  CONSTRAINT [FK_JobNotes_JobId] FOREIGN KEY([JobId])
REFERENCES [dbo].[Jobs] ([JobId])
GO

ALTER TABLE [dbo].[JobNotes] CHECK CONSTRAINT [FK_JobNotes_JobId]
GO


