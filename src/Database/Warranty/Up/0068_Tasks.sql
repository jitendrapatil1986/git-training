CREATE TABLE [dbo].[Tasks](
	[TaskId] [uniqueidentifier] NOT NULL,
	[EmployeeId] [uniqueidentifier] NOT NULL,
	[ReferenceId] [uniqueidentifier] NOT NULL,
	[Description] [varchar](255) NULL,
	[IsComplete] [bit] NULL,
	[TaskType] int NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [varchar](255) NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [varchar](255) NULL,	
 CONSTRAINT [PK_Tasks] PRIMARY KEY CLUSTERED 
(
	[TaskId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [ActiveData]
) ON [ActiveData]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Tasks] ADD  CONSTRAINT [DF_Tasks_Id]  DEFAULT (newsequentialid()) FOR [TaskId]
GO

ALTER TABLE [dbo].[Tasks]  WITH CHECK ADD  CONSTRAINT [FK_Tasks_EmployeeId] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employees] ([EmployeeId])
GO

ALTER TABLE [dbo].[Tasks] CHECK CONSTRAINT [FK_Tasks_EmployeeId]
GO