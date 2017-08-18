UPDATE CommunityAssignments 
SET CreatedDate = Cast('01-01-1753' as datetime)
Where CreatedDate Is Null

UPDATE CommunityAssignments
SET UpdatedDate = CreatedDate
Where UpdatedDate Is Null

UPDATE CommunityAssignments
SET AssignmentDate = UpdatedDate
Where AssignmentDate Is Null