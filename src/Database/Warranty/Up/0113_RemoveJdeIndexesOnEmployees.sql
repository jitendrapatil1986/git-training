IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IDX_Employees_JdeId')
BEGIN
    DROP INDEX IDX_Employees_JdeId ON Employees;
END

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IDX_Employees_JdeIdentifier')
BEGIN
    DROP INDEX IDX_Employees_JdeIdentifier ON Employees;
END