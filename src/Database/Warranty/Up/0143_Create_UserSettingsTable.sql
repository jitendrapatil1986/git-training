If dbo.TableExists('UserSettings') = 0
BEGIN
CREATE TABLE [dbo].[UserSettings] (
    [UserSettingsId] [UniqueIdentifier] NOT NULL,
    [EmployeeId] [UniqueIdentifier] NOT NULL,
    [ServiceCallWidgetSize] [int] NULL,
    [CreatedDate] [DATETIME2] NULL,
    [UpdatedDate] [DATETIME2] NULL,
    [CreatedBy] [VARCHAR](255) NULL,
    [UpdatedBy] [VARCHAR](255) NULL,
    CONSTRAINT PK_UserSettingsId
        PRIMARY KEY (UserSettingsId),
    CONSTRAINT [FK_ServiceCallComments_ServiceCallId]
        FOREIGN KEY (EmployeeId) REFERENCES Employees(EmployeeId)
);
END