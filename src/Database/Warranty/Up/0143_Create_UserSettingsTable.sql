If dbo.TableExists('UserSettings') = 0
BEGIN
CREATE TABLE UserSettings (
    UserSettingsId UniqueIdentifier,
    EmployeeId UniqueIdentifier,
    ServiceCallWidgetSize int ,
	UpdatedDate DATETIME,
    CONSTRAINT PK_UserSettingsId
        PRIMARY KEY (UserSettingsId),
    CONSTRAINT FK_ServiceCallComments_ServiceCallId
        FOREIGN KEY (EmployeeId) REFERENCES Employees(EmployeeId)
);
END