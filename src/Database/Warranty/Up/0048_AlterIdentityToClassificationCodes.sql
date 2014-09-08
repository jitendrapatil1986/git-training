--Create a temp table with an identity column
BEGIN TRANSACTION
GO

ALTER TABLE [lookups].ClassificationCodes
DROP CONSTRAINT [PK_ClassificationCodes]


CREATE TABLE [lookups].Tmp_ClassificationCodes(
	[ClassificationCodeId] [tinyint] NOT NULL IDENTITY (1, 1),
	[ClassificationCode] [varchar](50) NULL
 CONSTRAINT [PK_ClassificationCodes] PRIMARY KEY CLUSTERED 
(
	[ClassificationCodeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [ActiveData]
) ON [ActiveData]

SET IDENTITY_INSERT [lookups].Tmp_ClassificationCodes ON
GO

IF EXISTS (SELECT 1 FROM [lookups].ClassificationCodes)
INSERT INTO [lookups].Tmp_ClassificationCodes ([ClassificationCodeId], [ClassificationCode])
SELECT [ClassificationCodeId], [ClassificationCode] FROM [lookups].ClassificationCodes WITH (HOLDLOCK TABLOCKX)
GO

SET IDENTITY_INSERT [lookups].Tmp_ClassificationCodes OFF
GO

DROP TABLE [lookups].ClassificationCodes
GO

EXECUTE sp_rename N'[lookups].Tmp_ClassificationCodes', N'ClassificationCodes', 'OBJECT'
GO

COMMIT