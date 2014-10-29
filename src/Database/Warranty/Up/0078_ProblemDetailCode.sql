ALTER TABLE [lookups].[ProblemCodes]
ADD ProblemJDECode VARCHAR(10) NULL,
	ProblemDetailCode VARCHAR(255) NULL
;

ALTER TABLE [ServiceCallLineItems]
ADD ProblemJDECode VARCHAR(10) NULL,
	ProblemDetailCode VARCHAR(255) NULL
;
