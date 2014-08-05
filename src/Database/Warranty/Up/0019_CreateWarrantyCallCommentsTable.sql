CREATE TABLE WarrantyCallComments (
    WarrantyCallCommentId INT NOT NULL,
    WarrantyCallId INT NOT NULL,
    WarrantyCallComment VARCHAR(MAX)
)

ALTER TABLE WarrantyCallComments ADD CONSTRAINT PK_WarrantyCallComments
    PRIMARY KEY (WarrantyCallCommentId);

ALTER TABLE WarrantyCallComments ADD CONSTRAINT FK_WarrantyCallComments_WarrantyCallId
    FOREIGN KEY (WarrantyCallId) REFERENCES WarrantyCalls(WarrantyCallId);