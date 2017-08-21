-- view service calls with possibly wrong wsr

;with currentAssignments as (
    select *,
        rk = row_number() over (partition by communityId order by isnull(assignmentdate,'1987-04-09') desc)
    from CommunityAssignments
)
select 
    c.createdDate,
    c.servicecallId,
    assignedWsr = ec.EmployeeName,
    communityWsr = ea.EmployeeName,
    j.JobNumber,
    *
from ServiceCalls c
    inner join jobs j
        on j.jobid = c.jobid
    inner join currentAssignments a
        on a.communityId = j.communityId  
        and a.rk = 1
    inner join employees ec
        on ec.EmployeeId = c.WarrantyRepresentativeEmployeeId
    inner join employees ea
        on ea.EmployeeId = a.EmployeeId
where 1=1
    and c.CreatedDate > a.assignmentDate
    and c.WarrantyRepresentativeEmployeeId <> a.EmployeeId
order by c.CreatedDate desc


-- view a call's data

declare @serviceCallId uniqueidentifier = 'DB21B012-EDF8-423C-85E1-A7D2011A4061'

;with currentAssignments as (
    select *,
        rk = row_number() over (partition by communityId order by isnull(assignmentdate,'1987-04-09') desc)
    from CommunityAssignments
)
select 
    c.createdDate,
    c.servicecallId,
    assignedWsr = ec.EmployeeName,
    communityWsr = ea.EmployeeName,
    j.JobNumber,
    *
from ServiceCalls c
    inner join jobs j
        on j.jobid = c.jobid
    inner join currentAssignments a
        on a.communityId = j.communityId  
        and a.rk = 1
    inner join employees ec
        on ec.EmployeeId = c.WarrantyRepresentativeEmployeeId
    inner join employees ea
        on ea.EmployeeId = a.EmployeeId
where 1=1
    and c.ServiceCallId = @serviceCallId


-- update a call with the community wsr

declare @serviceCallId uniqueidentifier = 'DB21B012-EDF8-423C-85E1-A7D2011A4061'

begin tran
;with currentAssignments as (
    select *,
        rk = row_number() over (partition by communityId order by isnull(assignmentdate,'1987-04-09') desc)
    from CommunityAssignments
)
update c
set
    WarrantyRepresentativeEmployeeId = a.EmployeeId
from ServiceCalls c
    inner join jobs j
        on j.jobid = c.jobid
    inner join currentAssignments a
        on a.communityId = j.communityId  
        and a.rk = 1
where 1=1
    and c.ServiceCallId = @serviceCallId
    and c.WarrantyRepresentativeEmployeeId <> a.EmployeeId

rollback
--commit