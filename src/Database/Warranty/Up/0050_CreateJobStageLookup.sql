CREATE TABLE lookups.JobStages(
    JobStageId INT NOT NULL,
    JobStage varchar(100) NOT NULL,
    CONSTRAINT PK_LookUpJobStages 
        PRIMARY KEY (JobStageId)
)

INSERT INTO lookups.JobStages VALUES(0,'0');
INSERT INTO lookups.JobStages VALUES(1,'1');
INSERT INTO lookups.JobStages VALUES(2,'2');
INSERT INTO lookups.JobStages VALUES(3,'3');
INSERT INTO lookups.JobStages VALUES(4,'4');
INSERT INTO lookups.JobStages VALUES(5,'5');
INSERT INTO lookups.JobStages VALUES(6,'6');
INSERT INTO lookups.JobStages VALUES(7,'7');
INSERT INTO lookups.JobStages VALUES(8,'8');
INSERT INTO lookups.JobStages VALUES(9,'9');
INSERT INTO lookups.JobStages VALUES(10, 'Closed');