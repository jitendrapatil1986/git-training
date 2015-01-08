IF EXISTS(SELECT 1 FROM sys.indexes WHERE name = 'IDX_Jobs_JdeId')
    DROP INDEX IDX_Jobs_JdeId ON Jobs

IF EXISTS(SELECT 1 FROM sys.indexes WHERE name = 'IDX_Jobs_JobNumber')
    DROP INDEX IDX_Jobs_JobNumber ON Jobs

IF EXISTS(SELECT 1 FROM sys.indexes WHERE name = 'IDX_Payments_JdeId')
    DROP INDEX IDX_Payments_JdeId ON Payments;

IF EXISTS(SELECT 1 FROM sys.indexes WHERE name = 'IDX_Homeowners_JobId')
    DROP INDEX IDX_Homeowners_JobId ON Homeowners;

CREATE NONCLUSTERED INDEX IDX_Homeowners_JobId ON Homeowners(JobId);

IF EXISTS(SELECT 1 FROM sys.indexes WHERE name = 'IDX_ServiceCallAttachments_ServiceCallId')
    DROP INDEX IDX_ServiceCallAttachments_ServiceCallId ON ServiceCallAttachments;

CREATE NONCLUSTERED INDEX IDX_ServiceCallAttachments_ServiceCallId ON ServiceCallAttachments(ServiceCallId);

IF EXISTS(SELECT 1 FROM sys.indexes WHERE name = 'IDX_ServiceCallNotes_ServiceCallId')
    DROP INDEX IDX_ServiceCallNotes_ServiceCallId ON ServiceCallNotes;

CREATE NONCLUSTERED INDEX IDX_ServiceCallNotes_ServiceCallId ON ServiceCallNotes(ServiceCallId);

IF EXISTS(SELECT 1 FROM sys.indexes WHERE name = 'IDX_JobNotes_JobId')
    DROP INDEX IDX_JobNotes_JobId ON JobNotes;

CREATE NONCLUSTERED INDEX IDX_JobNotes_JobId ON JobNotes(JobId);

IF EXISTS(SELECT 1 FROM sys.indexes WHERE name = 'IDX_JobAttachments_JobId')
    DROP INDEX IDX_JobAttachments_JobId ON JobAttachments;

CREATE NONCLUSTERED INDEX IDX_JobAttachments_JobId ON JobAttachments(JobId);

IF EXISTS(SELECT 1 FROM sys.indexes WHERE name = 'IDX_JobVendorCostCodes_VendorIdJobId')
    DROP INDEX IDX_JobVendorCostCodes_VendorIdJobId ON JobVendorCostCodes;

CREATE NONCLUSTERED INDEX IDX_JobVendorCostCodes_VendorIdJobId ON JobVendorCostCodes(VendorId, JobId);

IF EXISTS(SELECT 1 FROM sys.indexes WHERE name = 'IDX_VendorPhones_VendorId')
    DROP INDEX IDX_VendorPhones_VendorId ON VendorPhones;

CREATE NONCLUSTERED INDEX IDX_VendorPhones_VendorId ON VendorPhones(VendorId);

IF EXISTS(SELECT 1 FROM sys.indexes WHERE name = 'IDX_VendorEmails_VendorId')
    DROP INDEX IDX_VendorEmails_VendorId ON VendorEmails;

CREATE NONCLUSTERED INDEX IDX_VendorEmails_VendorId ON VendorEmails(VendorId);
