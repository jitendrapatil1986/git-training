-- Inserting "AssignmentDate" column from CommunityAssignments to CommunityAssignmentHistory
INSERT INTO CommunityAssignmentHistory(AssignmentDate, EmployeeId, EmployeeAssignmentId, CommunityId, CreatedBy, CreatedDate)
Select AssignmentDate, EmployeeId, EmployeeAssignmentId, CommunityId, CreatedBy, CreatedDate from CommunityAssignments
 
--Removing the Column "AssignmentDate" from CommunityAssignments
 ALTER TABLE CommunityAssignments DROP COLUMN AssignmentDate, CreatedDate, CreatedBy