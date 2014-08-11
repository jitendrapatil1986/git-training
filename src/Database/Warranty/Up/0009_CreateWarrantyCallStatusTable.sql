CREATE TABLE lookups.WarrantyCallStatuses (
    WarrantyCallStatusId TINYINT NOT NULL,
    WarrantyCallStatus VARCHAR(20),
    CONSTRAINT PK_WarrantyCall 
        PRIMARY KEY (WarrantyCallStatusId)
);