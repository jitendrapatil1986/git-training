--Create a temp table with an identity column
BEGIN TRANSACTION
GO

--Delete constraint to ServiceCallStatuses
ALTER TABLE [dbo].[ServiceCalls] DROP CONSTRAINT [FK_Servicecalls_ServiceCallStatusId]
GO

ALTER TABLE [lookups].ServiceCallStatuses
DROP CONSTRAINT [PK_ServiceCallStatuses]


CREATE TABLE [lookups].Tmp_ServiceCallStatuses(
	[ServiceCallStatusId] [tinyint] NOT NULL IDENTITY (1, 1),
	[ServiceCallStatus] [varchar](20) NULL
 CONSTRAINT [PK_ServiceCallStatuses] PRIMARY KEY CLUSTERED 
(
	[ServiceCallStatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [ActiveData]
) ON [ActiveData]

SET IDENTITY_INSERT [lookups].Tmp_ServiceCallStatuses ON
GO

IF EXISTS (SELECT 1 FROM [lookups].ServiceCallStatuses)
INSERT INTO [lookups].Tmp_ServiceCallStatuses ([ServiceCallStatusId], [ServiceCallStatus])
SELECT [ServiceCallStatusId], [ServiceCallStatus] FROM [lookups].ServiceCallStatuses WITH (HOLDLOCK TABLOCKX)
GO

SET IDENTITY_INSERT [lookups].Tmp_ServiceCallStatuses OFF
GO

DROP TABLE [lookups].ServiceCallStatuses
GO

EXECUTE sp_rename N'[lookups].Tmp_ServiceCallStatuses', N'ServiceCallStatuses', 'OBJECT'
GO

--Re-add FK constraint to ServiceCallStatuses
ALTER TABLE [dbo].[ServiceCalls]  WITH CHECK ADD  CONSTRAINT [FK_Servicecalls_ServiceCallStatusId] FOREIGN KEY([ServiceCallStatusId])
REFERENCES [lookups].[ServiceCallStatuses] ([ServiceCallStatusId])
GO

ALTER TABLE [dbo].[ServiceCalls] CHECK CONSTRAINT [FK_Servicecalls_ServiceCallStatusId]
GO


COMMIT