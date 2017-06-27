If dbo.TableExists('UserSettings') IS NOT NULL
BEGIN

CREATE TABLE UserSettings (
    WidgetSize_Id int Identity(1, 1),
        
    EmployeeId UniqueIdentifier,

ServiceCallWidgetSize int ,
UpdatedDate DATETIME,
    
    CONSTRAINT PK_WidgetSize_Id
        PRIMARY KEY (WidgetSize_Id),
    CONSTRAINT FK_ServiceCallComments_ServiceCallId
        FOREIGN KEY (EmployeeId) REFERENCES Employees(EmployeeId)
);
END