ALTER TABLE Employees ADD JdeIdentifier VARCHAR(255) NULL;

CREATE NONCLUSTERED INDEX IDX_Employees_JdeId ON Employees(JdeIdentifier);
