CREATE TABLE lookups.ServiceCallStatuses (
    ServiceCallStatusId TINYINT NOT NULL,
    ServiceCallStatus VARCHAR(20),
    CONSTRAINT PK_ServiceCallStatuses
        PRIMARY KEY (ServiceCallStatusId)
);