-- Create the login used by the NSB Installer at runtime
-- This may already exists since other apps might have created it
IF NOT EXISTS (select 1 from master.dbo.syslogins where name = 'NSB')
BEGIN
	CREATE LOGIN [NSB] WITH PASSWORD = N'Gy*9d>VM&6gu6<k'
END
EXEC [{{DatabaseName}}_NServicebus].[sys].[sp_adduser] 'NSB', 'NServicebusInstaller', 'db_owner'
