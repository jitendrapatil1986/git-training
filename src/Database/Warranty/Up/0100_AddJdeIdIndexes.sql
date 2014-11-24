CREATE NONCLUSTERED INDEX IDX_WarrantyPayments_JdeIdentifier ON WarrantyPayments (JdeIdentifier);

CREATE NONCLUSTERED INDEX IDX_Payments_JdeIdentifier ON Payments (JdeIdentifier);

CREATE NONCLUSTERED INDEX IDX_Employees_JdeIdentifier ON Employees (JdeIdentifier);

CREATE NONCLUSTERED INDEX IDX_Jobs_JdeIdentifier ON Jobs (JdeIdentifier);

CREATE NONCLUSTERED INDEX IDX_Backcharges_JdeIdentifier ON Backcharges (JdeIdentifier);

CREATE NONCLUSTERED INDEX IDX_JobStages_JdeIdentifier ON JobStages (JdeIdentifier);

CREATE NONCLUSTERED INDEX IDX_PurchaseOrders_JdeIdentifier ON PurchaseOrders (JdeIdentifier);

CREATE NONCLUSTERED INDEX IDX_PurchaseOrderLineItems_JdeIdentifier ON PurchaseOrderLineItems (JdeIdentifier);
