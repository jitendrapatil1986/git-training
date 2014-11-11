
CREATE TABLE [dbo].[PurchaseOrders](
    [PurchaseOrderId] [uniqueidentifier] NOT NULL,
    [VendorNumber] [varchar](8) NULL,
	[VendorName] [varchar](255) NULL,
	[DeliveryInstructions] [varchar](255) NULL,
	[DeliveryDate] [datetime2](7) NULL,
	[CostCode] [varchar](255) NULL,
	[ObjectAccount] [varchar](255) NULL,
	[ServiceCallLineItemId] [uniqueidentifier] NULL,
    [JobNumber] [varchar](8) NULL,
    [JdeIdentifier] [varchar](255) NULL,
	[PurchaseOrderNote] [varchar](max) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [varchar](255) NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [varchar](255) NULL,
 CONSTRAINT [PK_PurchaseOrders] PRIMARY KEY CLUSTERED 
(
	[PurchaseOrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [ActiveData]
) ON [ActiveData]

GO

ALTER TABLE [dbo].[PurchaseOrders] ADD  CONSTRAINT [DF_PurchaseOrders_PurchaseOrderId]  DEFAULT (newsequentialid()) FOR [PurchaseOrderId]
GO

ALTER TABLE [dbo].[PurchaseOrders]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrders_ServiceCallLineItemId] FOREIGN KEY([ServiceCallLineItemId])
REFERENCES [dbo].[ServiceCallLineItems] ([ServiceCallLineItemId])
GO

ALTER TABLE [dbo].[PurchaseOrders] CHECK CONSTRAINT [FK_PurchaseOrders_ServiceCallLineItemId]
GO