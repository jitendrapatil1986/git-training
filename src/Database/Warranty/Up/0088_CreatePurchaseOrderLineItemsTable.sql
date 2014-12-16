
CREATE TABLE [dbo].[PurchaseOrderLineItems](
	[PurchaseOrderLineItemId] [uniqueidentifier] NOT NULL,
	[PurchaseOrderId] [uniqueidentifier] NULL,
	[LineNumber] [int] NULL,
	[Quantity] [decimal](18, 2) NULL,
	[UnitCost] [decimal](18, 2) NULL,
	[Description] [varchar](4000) NULL,
	[PurchaseOrderLineItemStatusId] [int] NULL,
	[JdeIdentifier] [varchar](255) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [varchar](255) NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [varchar](255) NULL,
 CONSTRAINT [PK_PurchaseOrderLineItems] PRIMARY KEY CLUSTERED 
(
	[PurchaseOrderLineItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [ActiveData]
) ON [ActiveData]

GO

ALTER TABLE [dbo].[PurchaseOrderLineItems] ADD  CONSTRAINT [DF_PurchaseOrderLineItems]  DEFAULT (newsequentialid()) FOR [PurchaseOrderLineItemId]
GO

ALTER TABLE [dbo].[PurchaseOrderLineItems]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrderLineItems_PurchaseOrderId] FOREIGN KEY([PurchaseOrderId])
REFERENCES [dbo].[PurchaseOrders] ([PurchaseOrderId])
GO

ALTER TABLE [dbo].[PurchaseOrderLineItems] CHECK CONSTRAINT [FK_PurchaseOrderLineItems_PurchaseOrderId]
GO


