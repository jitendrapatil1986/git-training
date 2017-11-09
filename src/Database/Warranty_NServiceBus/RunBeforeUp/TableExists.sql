if exists (select * from sys.objects where type = 'FN' and name = 'TableExists')
    drop function dbo.TableExists;
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