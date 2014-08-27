CREATE TABLE lookups.ServiceCallTypes (
    ServiceCallTypeId TINYINT NOT NULL,
    ServiceCallType VARCHAR(50) NOT NULL,
    CONSTRAINT PK_ServiceCallType
        PRIMARY KEY(ServiceCallTypeId)
)

INSERT INTO lookups.ServiceCallTypes(ServiceCallTypeId, ServiceCallType) VALUES (1, 'HBR5')
INSERT INTO lookups.ServiceCallTypes(ServiceCallTypeId, ServiceCallType) VALUES (2, 'Home Furnishings Warranty')
INSERT INTO lookups.ServiceCallTypes(ServiceCallTypeId, ServiceCallType) VALUES (3, 'HVAC Goodwill')
INSERT INTO lookups.ServiceCallTypes(ServiceCallTypeId, ServiceCallType) VALUES (4, 'Mold Inspection')
INSERT INTO lookups.ServiceCallTypes(ServiceCallTypeId, ServiceCallType) VALUES (5, 'Presettlement List')
INSERT INTO lookups.ServiceCallTypes(ServiceCallTypeId, ServiceCallType) VALUES (6, 'Proactive Call')
INSERT INTO lookups.ServiceCallTypes(ServiceCallTypeId, ServiceCallType) VALUES (7, 'Proactive Inspection')
INSERT INTO lookups.ServiceCallTypes(ServiceCallTypeId, ServiceCallType) VALUES (8, 'Re-sale Inspection List')
INSERT INTO lookups.ServiceCallTypes(ServiceCallTypeId, ServiceCallType) VALUES (9, 'Request on Hold')
INSERT INTO lookups.ServiceCallTypes(ServiceCallTypeId, ServiceCallType) VALUES (10, 'Twelve Month List')
INSERT INTO lookups.ServiceCallTypes(ServiceCallTypeId, ServiceCallType) VALUES (11, 'Warranty Orientation')
INSERT INTO lookups.ServiceCallTypes(ServiceCallTypeId, ServiceCallType) VALUES (12, 'Warranty Service Request')
