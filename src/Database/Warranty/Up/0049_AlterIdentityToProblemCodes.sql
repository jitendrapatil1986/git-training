--Create a temp table with an identity column
BEGIN TRANSACTION
GO

ALTER TABLE [lookups].ProblemCodes
DROP CONSTRAINT [PK_ProblemCodes]


CREATE TABLE [lookups].Tmp_ProblemCodes(
	[ProblemCodeId] [int] NOT NULL IDENTITY (1, 1),
	[ProblemCode] [varchar](250) NULL
 CONSTRAINT [PK_ProblemCodes] PRIMARY KEY CLUSTERED 
(
	[ProblemCodeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [ActiveData]
) ON [ActiveData]

SET IDENTITY_INSERT [lookups].Tmp_ProblemCodes ON
GO

IF EXISTS (SELECT 1 FROM [lookups].ProblemCodes)
INSERT INTO [lookups].Tmp_ProblemCodes ([ProblemCodeId], [ProblemCode])
SELECT [ProblemCodeId], [ProblemCode] FROM [lookups].ProblemCodes WITH (HOLDLOCK TABLOCKX)
GO

SET IDENTITY_INSERT [lookups].Tmp_ProblemCodes OFF
GO

DROP TABLE [lookups].ProblemCodes
GO

EXECUTE sp_rename N'[lookups].Tmp_ProblemCodes', N'ProblemCodes', 'OBJECT'
GO

COMMIT