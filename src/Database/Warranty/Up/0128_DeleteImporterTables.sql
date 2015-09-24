IF OBJECT_ID('imports.ArchivedVendorsStage', 'U') IS NOT NULL
	DROP TABLE imports.ArchivedVendorsStage;
GO

IF OBJECT_ID('imports.CommunityList', 'U') IS NOT NULL
	DROP TABLE imports.CommunityList;
GO

IF OBJECT_ID('imports.CommunityStage', 'U') IS NOT NULL
	DROP TABLE imports.CommunityStage;
GO

IF OBJECT_ID('imports.CustomerImports', 'U') IS NOT NULL
	DROP TABLE imports.CustomerImports;
GO

IF OBJECT_ID('imports.JobOptionImports', 'U') IS NOT NULL
	DROP TABLE imports.JobOptionImports;
GO

IF OBJECT_ID('imports.JobStage', 'U') IS NOT NULL
	DROP TABLE imports.JobStage;
GO

IF OBJECT_ID('imports.JobStageImports', 'U') IS NOT NULL
	DROP TABLE imports.JobStageImports;
GO

IF OBJECT_ID('imports.MasterCommunityImports', 'U') IS NOT NULL
	DROP TABLE imports.MasterCommunityImports;
GO

IF OBJECT_ID('imports.PaymentStage', 'U') IS NOT NULL
	DROP TABLE imports.PaymentStage;
GO

IF OBJECT_ID('imports.ServiceCallImports', 'U') IS NOT NULL
	DROP TABLE imports.ServiceCallImports;
GO

IF OBJECT_ID('imports.tmp_MasterCommunityImports', 'U') IS NOT NULL
	DROP TABLE imports.tmp_MasterCommunityImports;
GO

IF OBJECT_ID('imports.WarrantyPaymentStage', 'U') IS NOT NULL
	DROP TABLE imports.WarrantyPaymentStage;
GO