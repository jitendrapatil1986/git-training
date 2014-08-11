CREATE TABLE ServiceCallLineItems (
    ServiceCallLineItemId UNIQUEIDENTIFIER NOT NULL CONSTRAINT 
        DF_ServiceCallLineItems DEFAULT NEWSEQUENTIALID(),
    ServiceCallId UNIQUEIDENTIFIER,
    LineNumber INT,
    ProblemCode VARCHAR(4000),
    ProblemDescription VARCHAR(4000),
    CauseDescription VARCHAR(4000),
    ClassificationNote VARCHAR(4000),
    LineItemRoot VARCHAR(4000),
    Completed BIT,
    CreatedDate DATETIME2,
    CreatedBy VARCHAR(255),
    UpdatedDate DATETIME2,
    UpdatedBy VARCHAR(255),
    CONSTRAINT PK_ServiceCallLineItems
        PRIMARY KEY (ServiceCallLineItemId),
    CONSTRAINT FK_ServiceCallLineItems_ServiceCallId
        FOREIGN KEY (ServiceCallId) REFERENCES ServiceCalls(ServiceCallId)
);