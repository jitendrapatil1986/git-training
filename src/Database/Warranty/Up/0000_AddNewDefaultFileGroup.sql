DECLARE @DatabaseName NVARCHAR(256) = 'Warranty';

IF DB_NAME() LIKE @DatabaseName + '%'
BEGIN
    SET @DatabaseName = DB_NAME();

    DECLARE @EnsureSimpleMode    NVARCHAR(MAX) = 'ALTER DATABASE [' + DB_NAME() + '] SET RECOVERY FULL;
                                                  ALTER DATABASE [' + DB_NAME() + '] SET RECOVERY SIMPLE;';
    DECLARE @CreateFileGroup     NVARCHAR(MAX) = 'ALTER DATABASE [' + DB_NAME() + '] ADD FILEGROUP ActiveData;';
    DECLARE @SetDefaultFileGroup NVARCHAR(MAX) = 'ALTER DATABASE [' + DB_NAME() + '] MODIFY FILEGROUP ActiveData DEFAULT;';
    DECLARE @MasterFile          NVARCHAR(MAX);
    DECLARE @NewFile             NVARCHAR(MAX);
    DECLARE @LogFile             NVARCHAR(MAX);

    IF DB_NAME() LIKE '%Test'
    BEGIN
        SET @MasterFile = (SELECT TOP 1 name 
                                FROM master.sys.master_files 
                                WHERE DB_NAME(database_id) = @DatabaseName 
                                    AND type_desc = 'ROWS' 
                                    ORDER BY file_id);

        SET @NewFile = (SELECT TOP 1 SUBSTRING(physical_name, 0, charindex(name, physical_name, 0)) 
                            FROM master.sys.master_files 
                            WHERE DB_NAME(database_id) = @DatabaseName) 
                        + @DatabaseName +'_ActiveData_File_01.ndf';

        SET @LogFile = (SELECT TOP 1 name 
                            FROM master.sys.master_files 
                            WHERE DB_NAME(database_id) = @DatabaseName 
                            AND type_desc = 'LOG' 
                            ORDER BY file_id);
    END
    ELSE IF DB_NAME() LIKE '%Training'
    BEGIN
        SET @MasterFile = (SELECT TOP 1 name 
                                FROM master.sys.master_files 
                                WHERE DB_NAME(database_id) = @DatabaseName 
                                AND type_desc = 'ROWS' 
                                ORDER BY file_id);

        SET @NewFile = (SELECT TOP 1 SUBSTRING(physical_name, 0, charindex(name, physical_name, 0)) 
                            FROM master.sys.master_files 
                            WHERE DB_NAME(database_id) = @DatabaseName) 
                        + @DatabaseName +'_ActiveData_File_01.ndf';

        SET @LogFile = (SELECT TOP 1 name 
                            FROM master.sys.master_files 
                            WHERE DB_NAME(database_id) = @DatabaseName 
                            AND type_desc = 'LOG' 
                            ORDER BY file_id);
    END
    ELSE
    BEGIN
        SET @MasterFile = (SELECT TOP 1 name 
                                FROM master.sys.master_files 
                                WHERE DB_NAME(database_id) = @DatabaseName 
                                AND type_desc = 'ROWS' 
                                ORDER BY file_id);

        SET @NewFile = (SELECT TOP 1 SUBSTRING(physical_name, 0, charindex(name, physical_name, 0)) 
                            FROM master.sys.master_files 
                            WHERE DB_NAME(database_id) = @DatabaseName) 
                        + @DatabaseName + '_ActiveData_File_01.ndf';

        SET @LogFile = (SELECT TOP 1 name 
                            FROM master.sys.master_files 
                            WHERE DB_NAME(database_id) = @DatabaseName 
                            AND type_desc = 'LOG' 
                            ORDER BY file_id);
    END

    DECLARE @AddFile NVARCHAR(MAX) = 'ALTER DATABASE [' + DB_NAME() + '] ADD FILE 
                                        ( NAME=''ActiveData_File_01''
                                        , FILENAME=''' + @NewFile + '''
                                        , SIZE = 128MB
                                        , FILEGROWTH = 64MB
                                        ) TO FILEGROUP ActiveData;';

    DECLARE @SetMasterFileProps NVARCHAR(MAX) = 'ALTER DATABASE [' + DB_NAME() + '] MODIFY FILE 
                                                    ( NAME=''' + @MasterFile + '''
                                                    , SIZE = 128MB
                                                    , FILEGROWTH = 64MB
                                                    );';

    DECLARE @SetLogFileProps NVARCHAR(MAX) = 'ALTER DATABASE [' + DB_NAME() + '] MODIFY FILE 
                                                    ( NAME=''' + @LogFile + '''
                                                    , SIZE = 128MB
                                                    , FILEGROWTH = 128MB
                                                    );';

    exec sp_executesql @EnsureSimpleMode;
    exec sp_executesql @CreateFileGroup;
    exec sp_executesql @AddFile;
    exec sp_executesql @SetDefaultFileGroup;
    exec sp_executesql @SetMasterFileProps;
    exec sp_executesql @SetLogFileProps;
END