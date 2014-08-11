CREATE TABLE Employees (
    EmployeeId UNIQUEIDENTIFIER NOT NULL CONSTRAINT
        DF_Employees_Id DEFAULT NEWSEQUENTIALID(),
    EmployeeNumber VARCHAR(8),
    EmployeeName VARCHAR(100),
    CreatedDate DATETIME2,
    CreatedBy VARCHAR(255),
    UpdatedDate DATETIME2,
    UpdatedBy VARCHAR(255),
    CONSTRAINT PK_Employees
        PRIMARY KEY (EmployeeId)
);