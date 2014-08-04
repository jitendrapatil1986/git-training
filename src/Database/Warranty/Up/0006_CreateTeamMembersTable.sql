CREATE TABLE TeamMembers (
    TeamMemberId INT NOT NULL,
    TeamMemberNumber VARCHAR(8),
    TeamMemberName VARCHAR(100),
    Fax VARCHAR(30),
    CreatedDate DATETIME2,
    CreatedBy VARCHAR(255),
    UpdatedDate DATETIME2,
    UpdatedBy VARCHAR(255)
);

ALTER TABLE TeamMembers ADD CONSTRAINT PK_TeamMembers
    PRIMARY KEY (TeamMemberId);
