EXEC sp_rename 'HomeOwners', 'Homeowners';
EXEC sp_rename 'Homeowners.HomeOwnerId', 'HomeownerId', 'COLUMN';
EXEC sp_rename 'Homeowners.HomeOwnerName', 'HomeownerName', 'COLUMN';
EXEC sp_rename 'Homeowners.HomeOwnerNumber', 'HomeownerNumber', 'COLUMN';
EXEC sp_rename 'Jobs.CurrentHomeOwnerId', 'CurrentHomeownerId', 'COLUMN';
EXEC sp_rename 'ServiceCalls.HomeOwnerSignature', 'HomeownerSignature', 'COLUMN';
EXEC sp_rename 'imports.CustomerImports.HomeOwner', 'Homeowner', 'COLUMN';
EXEC sp_rename 'imports.CustomerImports.HomeOwnerMoved', 'HomeownerMoved', 'COLUMN';
