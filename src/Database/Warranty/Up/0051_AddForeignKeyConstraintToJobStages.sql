ALTER TABLE JobStages ADD CONSTRAINT FK_JobStageId
    FOREIGN KEY(JobStageId) REFERENCES lookups.JobStages(JobStageId)