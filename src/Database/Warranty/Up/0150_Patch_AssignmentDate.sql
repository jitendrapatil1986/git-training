-- Inserting "AssignmentDate" column from CommunityAssignments to CommunityAssignmentHistory
INSERT INTO CommunityAssignmentHistory(AssignmentDate)
Select AssignmentDate from CommunityAssignments
 
--Removing the Column "AssignmentDate" from CommunityAssignments
 ALTER TABLE CommunityAssignments DROP COLUMN AssignmentDate