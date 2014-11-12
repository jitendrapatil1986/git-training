
DELETE FROM [lookups].ProblemCodeRootCauses WHERE 1=1;

DROP TABLE [lookups].ProblemCodeRootCauses;
Go
DELETE FROM [lookups].[ProblemCodes] WHERE 1=1

DROP TABLE [lookups].[ProblemCodes]
Go

CREATE TABLE [dbo].[ProblemCodes](
	ProblemCodeId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT(NewSequentialId()),
	JdeCode VARCHAR(10),
	CategoryCode VARCHAR(255),
	DetailCode VARCHAR(255),
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [varchar](255) NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [varchar](255) NULL
);
Go

ALTER TABLE [ServiceCallLineItems]
ADD ProblemJDECode VARCHAR(10) NULL,
	ProblemDetailCode VARCHAR(255) NULL
;
