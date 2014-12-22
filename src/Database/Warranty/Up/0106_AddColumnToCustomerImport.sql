IF NOT EXISTS (SELECT 1 FROM sys.columns c INNER JOIN sys.tables t ON c.object_id = t.object_id and t.name = 'CustomerImports' and c.name = 'FileNames')
BEGIN
    ALTER TABLE imports.CustomerImports ADD FileNames VARCHAR(4000);
END
