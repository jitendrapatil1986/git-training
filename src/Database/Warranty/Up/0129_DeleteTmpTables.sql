IF OBJECT_ID('dbo.tmp_ActiveDivisions', 'U') IS NOT NULL
	DROP TABLE dbo.tmp_ActiveDivisions;
GO

IF OBJECT_ID('dbo.tmp_CommunityAssignments', 'U') IS NOT NULL
	DROP TABLE dbo.tmp_CommunityAssignments;
GO

IF OBJECT_ID('dbo.tmp_deletedEmployees', 'U') IS NOT NULL
	DROP TABLE dbo.tmp_deletedEmployees;
GO

IF OBJECT_ID('dbo.tmp_F0006', 'U') IS NOT NULL
	DROP TABLE dbo.tmp_F0006;
GO

IF OBJECT_ID('dbo.tmp_f0101_employees', 'U') IS NOT NULL
	DROP TABLE dbo.tmp_f0101_employees;
GO

IF OBJECT_ID('dbo.tmp_Tasks', 'U') IS NOT NULL
	DROP TABLE dbo.tmp_Tasks;
GO