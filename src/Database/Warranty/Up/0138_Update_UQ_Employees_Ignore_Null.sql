IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME='UQ_Employees_Number')
	ALTER TABLE [dbo].[Employees] DROP CONSTRAINT [UQ_Employees_Number]

IF EXISTS(SELECT 1 FROM sys.indexes WHERE name='UQ_Employees_Number' AND object_id = OBJECT_ID('dbo.Employees'))
	DROP INDEX [UQ_Employees_Number] ON [dbo].[Employees]

CREATE UNIQUE NONCLUSTERED INDEX [UQ_Employees_Number]
ON [dbo].[Employees] (EmployeeNumber)
WHERE EmployeeNumber IS NOT NULL;