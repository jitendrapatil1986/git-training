if dbo.ColumnExists('Payments', 'SendCheckToPC') = 0
begin
    ALTER TABLE Payments ADD SendCheckToPC bit DEFAULT 0;
end
go