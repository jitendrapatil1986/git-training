CREATE TABLE [lookups].[ProblemCodeRootCauses](
	[ProblemCodeId] [int] NOT NULL,
	[RootCauseId] TINYINT NOT NULL,
	CONSTRAINT [PK_ProblemCodeRootCausess] PRIMARY KEY 
	(
		[ProblemCodeId] ASC,
		[RootCauseId] ASC
	),
	CONSTRAINT [FK_ProblemCodeRootCauses_ProblemCodeId]
		FOREIGN KEY ([ProblemCodeId]) REFERENCES [lookups].[ProblemCodes] ([ProblemCodeId]),
	
	CONSTRAINT [FK_ProblemCodeRootCauses_RootCauseId] 
		FOREIGN KEY([RootCauseId]) REFERENCES [lookups].[RootCauses] ([RootCauseId])
);
