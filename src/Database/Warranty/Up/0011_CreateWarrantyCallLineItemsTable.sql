CREATE TABLE WarrantyCallLineItems (
    WarrantyCallLineItemId UNIQUEIDENTIFIER NOT NULL,
    WarrantyCallId UNIQUEIDENTIFIER,
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
    UpdatedBy VARCHAR(255)
);

ALTER TABLE WarrantyCallLineItems ADD CONSTRAINT DF_WarrantyCallLineItems
    DEFAULT NEWSEQUENTIALID() FOR WarrantyCallLineItemId;

ALTER TABLE WarrantyCallLineItems ADD CONSTRAINT PK_WarrantyCallLineItems
    PRIMARY KEY (WarrantyCallLineItemId);

ALTER TABLE WarrantyCallLineItems ADD CONSTRAINT FK_WarrantyCallLineItems_WarrantyCallId
    FOREIGN KEY (WarrantyCallId) REFERENCES WarrantyCalls(WarrantyCallId);