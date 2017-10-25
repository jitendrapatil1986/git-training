if object_id('tempdb..#historyIds') is not null
begin
    drop table #historyIds
end
create table #historyIds (CommunityId uniqueidentifier, EmployeeId uniqueidentifier, CreatedBy varchar(255), CreatedDate datetime2)

;with assignmentsRankedByDate as (
select *,
rankByDate = row_number() over (partition by CommunityId order by UpdatedDate desc)
from communityAssignments
)

insert into #historyIds (CommunityId, EmployeeId, CreatedBy, CreatedDate) 
select CommunityId, EmployeeId, CreatedBy, CreatedDate
from assignmentsRankedByDate
where rankByDate > 1

if object_id('CommunityAssignmentHistory') is not null
begin
    Truncate table CommunityAssignmentHistory
end
Insert into CommunityAssignmentHistory(CommunityId, EmployeeId, CreatedBy, CreatedDate)
Select CommunityId, EmployeeId, CreatedBy, CreatedDate from #historyIds

Delete from CommunityAssignments where CommunityId In (Select CommunityId from #historyIds)


