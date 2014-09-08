--Create a temp table with an identity column
BEGIN TRANSACTION
GO

ALTER TABLE [lookups].RootCauses
DROP CONSTRAINT [PK_RootCauses]


CREATE TABLE [lookups].Tmp_RootCauses(
	[RootCauseId] [tinyint] NOT NULL IDENTITY (1, 1),
	[RootCause] [varchar](50) NULL
 CONSTRAINT [PK_RootCauses] PRIMARY KEY CLUSTERED 
(
	[RootCauseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [ActiveData]
) ON [ActiveData]

SET IDENTITY_INSERT [lookups].Tmp_RootCauses ON
GO

IF EXISTS (SELECT 1 FROM [lookups].RootCauses)
INSERT INTO [lookups].Tmp_RootCauses ([RootCauseId], [RootCause])
SELECT [RootCauseId], [RootCause] FROM [lookups].RootCauses WITH (HOLDLOCK TABLOCKX)
GO

SET IDENTITY_INSERT [lookups].Tmp_RootCauses OFF
GO

DROP TABLE [lookups].RootCauses
GO

EXECUTE sp_rename N'[lookups].Tmp_RootCauses', N'RootCauses', 'OBJECT'
GO

COMMIT