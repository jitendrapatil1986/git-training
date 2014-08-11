CREATE TABLE WarrantyCallComments (
    WarrantyCallCommentId UNIQUEIDENTIFIER NOT NULL CONSTRAINT
        WarrantyCallComments_Id DEFAULT NEWSEQUENTIALID(),
    WarrantyCallId UNIQUEIDENTIFIER NOT NULL,
    WarrantyCallComment VARCHAR(MAX),
    CreatedDate DATETIME2,
    CreatedBy VARCHAR(255),
    UpdatedDate DATETIME2,
    UpdatedBy VARCHAR(255),
    CONSTRAINT PK_WarrantyCallComments
        PRIMARY KEY (WarrantyCallCommentId),
    CONSTRAINT FK_WarrantyCallComments_WarrantyCallId
        FOREIGN KEY (WarrantyCallId) REFERENCES WarrantyCalls(WarrantyCallId)
);