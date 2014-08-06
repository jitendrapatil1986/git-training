CREATE TABLE WarrantyCallComments (
    WarrantyCallCommentId UNIQUEIDENTIFIER NOT NULL,
    WarrantyCallId UNIQUEIDENTIFIER NOT NULL,
    WarrantyCallComment VARCHAR(MAX),
    CreatedDate DATETIME2,
    CreatedBy VARCHAR(255),
    UpdatedDate DATETIME2,
    UpdatedBy VARCHAR(255)
)

ALTER TABLE WarrantyCallComments ADD CONSTRAINT DF_WarrantyCallComments_Id
    DEFAULT NEWSEQUENTIALID() FOR WarrantyCallCommentId;

ALTER TABLE WarrantyCallComments ADD CONSTRAINT PK_WarrantyCallComments
    PRIMARY KEY (WarrantyCallCommentId);

ALTER TABLE WarrantyCallComments ADD CONSTRAINT FK_WarrantyCallComments_WarrantyCallId
    FOREIGN KEY (WarrantyCallId) REFERENCES WarrantyCalls(WarrantyCallId);