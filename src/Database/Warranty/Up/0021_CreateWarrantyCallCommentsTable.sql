CREATE TABLE ServiceCallComments (
    ServiceCallCommentId UNIQUEIDENTIFIER NOT NULL CONSTRAINT
        ServiceCallComments_Id DEFAULT NEWSEQUENTIALID(),
    ServiceCallId UNIQUEIDENTIFIER NOT NULL,
    ServiceCallComment VARCHAR(MAX),
    CreatedDate DATETIME2,
    CreatedBy VARCHAR(255),
    UpdatedDate DATETIME2,
    UpdatedBy VARCHAR(255),
    CONSTRAINT PK_ServiceCallComments
        PRIMARY KEY (ServiceCallCommentId),
    CONSTRAINT FK_ServiceCallComments_ServiceCallId
        FOREIGN KEY (ServiceCallId) REFERENCES ServiceCalls(ServiceCallId)
);