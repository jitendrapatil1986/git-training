if dbo.TableExists('ApprovedShowcasesHealthCheckSagaData') = 1
begin
	drop table dbo.ApprovedShowcasesHealthCheckSagaData
end

if dbo.TableExists('HEALTH_JobsMissingHomeOwnerInfo')  = 1
begin
	drop table dbo.HEALTH_JobsMissingHomeOwnerInfo
end

if dbo.TableExists('HEALTH_Showcase') = 1
begin
	drop table dbo.HEALTH_Showcase
end

if dbo.TableExists('HEALTH_SoldJob') = 1
begin
	drop table dbo.HEALTH_SoldJob
end

if dbo.TableExists('JobsMissingHomeOwnerInfoHealthCheckSagaData') = 1
begin
	drop table dbo.JobsMissingHomeOwnerInfoHealthCheckSagaData
end

if dbo.TableExists('SoldJobsHealthCheckSagaData') = 1
begin
	drop table dbo.SoldJobsHealthCheckSagaData
end

