ALTER TABLE [PurchaseOrders]
DROP COLUMN [CostCode]
GO

ALTER TABLE [PurchaseOrders]
ADD [CostCode] [int] null
GO