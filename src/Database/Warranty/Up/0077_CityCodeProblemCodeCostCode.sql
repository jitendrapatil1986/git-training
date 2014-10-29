CREATE TABLE [CityCodeProblemCodeCostCodes](
	[CityCodeProblemCodeCostCodeId] UNIQUEIDENTIFIER NOT NULL,
	[CityCode] VARCHAR(10) NULL,
	[ProblemJDECode] VARCHAR(10) NULL,
	[CostCode] VARCHAR(10) NULL,
	[CreatedDate] DATETIME2(7) NULL,
	[CreatedBy] VARCHAR(255) NULL,
	[ModifiedDate] DATETIME2(7) NULL,
	[ModifiedBy] VARCHAR(255) NULL
	CONSTRAINT PK_CityCodeProblemCodeCostCodes
		PRIMARY KEY(CityCodeProblemCodeCostCodeId)
);