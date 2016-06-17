if exists (
    select *
    from sys.objects
    where type = 'FN'
      and name = 'FunctionExists'
)
begin
    drop function dbo.FunctionExists;
end
go

create function dbo.FunctionExists(@functionName sysname) returns bit
as
begin
    declare @x bit;
    select @x = case
        when exists (
            select *
            from sys.objects
            where type = 'FN'
                and name = @functionName
        )
        then 1
        else 0
    end
    return @x;
end
go


if dbo.FunctionExists('ProcedureExists') = 1
begin
    drop function dbo.ProcedureExists;
end
go

create function dbo.ProcedureExists(@procedureName sysname) returns bit
as
begin
    declare @x bit;
    select @x = case
        when exists (
            select *
            from sys.procedures
            where name = @procedureName
        )
        then 1
        else 0
    end
    return @x;
end
go

if dbo.FunctionExists('TableExists') = 1
begin
    drop function dbo.TableExists;
end
go

create function dbo.TableExists(@tableName sysname) returns bit
as
begin
    declare @x bit;
    select @x = case
        when exists (
            select *
            from sys.tables
            where name = @tableName
        )
        then 1
        else 0
    end
    return @x;
end
go

if dbo.FunctionExists('ColumnExists') = 1
begin
    drop function dbo.ColumnExists;
end
go

create function dbo.ColumnExists(@tableName sysname, @columnName sysname) returns bit
as
begin
    declare @x bit;
    select @x = case
        when exists (
            select *
            from sys.columns
            where object_id = object_id(@tableName)
                and name = @columnName
        )
        then 1
        else 0
    end
    return @x;
end
go

if dbo.FunctionExists('ViewExists') = 1
begin
    drop function dbo.ViewExists;
end
go

create function dbo.ViewExists(@viewName sysname) returns bit
as
begin
    declare @x bit;
    select @x = case
        when exists (
            select *
            from sys.views
            where name = @viewName
        )
        then 1
        else 0
    end
    return @x;
end
go

if dbo.FunctionExists('IndexExists') = 1
begin
    drop function dbo.IndexExists;
end
go

create function dbo.IndexExists(@parentName sysname, @indexName sysname) returns bit
as
begin
    declare @x bit;
    select @x = case
        when exists (
            select *
            from sys.indexes
            where object_id = object_id(@parentName)
                and name = @indexName
        )
        then 1
        else 0
    end
    return @x;
end
go

if dbo.FunctionExists('ConstraintExists') = 1
begin
    drop function dbo.ConstraintExists;
end
go

create function dbo.ConstraintExists(@parentName sysname, @constraintName sysname) returns bit
as
begin
    declare @x bit;
    select @x = case
        when exists (
            select *
            from sys.check_constraints
            where parent_object_id = object_id(@parentName)
                and name = @constraintName
        )
        then 1
        else 0
    end
    return @x;
end
go

