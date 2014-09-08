--Create a temp table with an identity column
BEGIN TRANSACTION
GO

ALTER TABLE [lookups].ServiceCallTypes
DROP CONSTRAINT [PK_ServiceCallType]


CREATE TABLE [lookups].Tmp_ServiceCallTypes(
	[ServiceCallTypeId] [tinyint] NOT NULL IDENTITY (1, 1),
	[ServiceCallType] [varchar](50) NULL
 CONSTRAINT [PK_ServiceCallType] PRIMARY KEY CLUSTERED 
(
	[ServiceCallTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [ActiveData]
) ON [ActiveData]

SET IDENTITY_INSERT [lookups].Tmp_ServiceCallTypes ON
GO

IF EXISTS (SELECT 1 FROM [lookups].ServiceCallTypes)
INSERT INTO [lookups].Tmp_ServiceCallTypes ([ServiceCallTypeId], [ServiceCallType])
SELECT [ServiceCallTypeId], [ServiceCallType] FROM [lookups].ServiceCallTypes WITH (HOLDLOCK TABLOCKX)
GO

SET IDENTITY_INSERT [lookups].Tmp_ServiceCallTypes OFF
GO

DROP TABLE [lookups].ServiceCallTypes
GO

EXECUTE sp_rename N'[lookups].Tmp_ServiceCallTypes', N'ServiceCallTypes', 'OBJECT'
GO

COMMIT