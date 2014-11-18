ALTER TABLE Cities ADD 
CONSTRAINT UQ_Cities_Code UNIQUE (CityCode);

ALTER TABLE Divisions ADD
CONSTRAINT UQ_Divisions_Code UNIQUE (DivisionCode);

ALTER TABLE Projects ADD
CONSTRAINT UQ_Projects_Number UNIQUE (ProjectNumber);

ALTER TABLE Communities ADD
CONSTRAINT UQ_Communities_Number UNIQUE (CommunityNumber);

ALTER TABLE Employees ADD
CONSTRAINT UQ_Employees_Number UNIQUE (EmployeeNumber);

ALTER TABLE Homeowners ADD
CONSTRAINT UQ_Homeowners_JobNumber UNIQUE (JobId, HomeownerNumber);

ALTER TABLE Jobs ADD
CONSTRAINT UQ_Jobs_Number UNIQUE (JobNumber);
