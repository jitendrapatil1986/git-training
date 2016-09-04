if dbo.ColumnExists('Payments', 'NotifiedPC') = 0
begin
    ALTER TABLE Payments ADD NotifiedPC varchar(MAX) DEFAULT '';
end
go