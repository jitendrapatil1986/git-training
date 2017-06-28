if dbo.ColumnExists('CommunityAssignments', 'AssignmentDate') = 0
begin
ALTER TABLE [CommunityAssignments]
ADD [AssignmentDate] datetime2(7) null
end
go
