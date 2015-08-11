ALTER TABLE CommunityAssignments
ADD CreatedDate DATETIME2 NULL
GO

ALTER TABLE CommunityAssignments
ADD CreatedBy VARCHAR(255) NULL
GO

ALTER TABLE CommunityAssignments
ADD UpdatedDate DATETIME2 NULL
GO

ALTER TABLE CommunityAssignments
ADD UpdatedBy VARCHAR(255) NULL
GO