CREATE TABLE Employees (
    EmployeeId INT NOT NULL,
    EmployeeNumber VARCHAR(8),
    EmployeeName VARCHAR(100),
    CreatedDate DATETIME2,
    CreatedBy VARCHAR(255),
    UpdatedDate DATETIME2,
    UpdatedBy VARCHAR(255)
);

ALTER TABLE Employees ADD CONSTRAINT PK_Employees
    PRIMARY KEY (EmployeeId);
