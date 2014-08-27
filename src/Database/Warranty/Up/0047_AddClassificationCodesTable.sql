CREATE TABLE lookups.ClassificationCodes (
    ClassificationCodeId TINYINT NOT NULL,
    ClassificationCode VARCHAR(30) NOT NULL,
    CONSTRAINT PK_ClassificationCodes
        PRIMARY KEY (ClassificationCodeId)
);

INSERT INTO lookups.ClassificationCodes (ClassificationCodeId, ClassificationCode) VALUES (1 ,'No Action')
INSERT INTO lookups.ClassificationCodes (ClassificationCodeId, ClassificationCode) VALUES (2 ,'Builder Incomplete')
INSERT INTO lookups.ClassificationCodes (ClassificationCodeId, ClassificationCode) VALUES (3 ,'Customer Goodwill')
INSERT INTO lookups.ClassificationCodes (ClassificationCodeId, ClassificationCode) VALUES (4 ,'Warrantable')