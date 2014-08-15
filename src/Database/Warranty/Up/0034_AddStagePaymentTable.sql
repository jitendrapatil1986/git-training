CREATE TABLE imports.PaymentStage (
    [Id] [int] IDENTITY(-200000000, 1),
	[VendorNumber] [varchar](8) NULL,
	[Amount] [decimal](18, 2) NULL,
	[PaymentStatus] [varchar](10) NULL,
	[JobNumber] [varchar](20) NULL,
	[JdeIdentifier] [varchar](255) NULL,
    [CreatedDate] [datetime2] NULL,
    [CreatedBy] [varchar](255) NULL,
    [RequestedDate] [datetime2] NULL,
    CONSTRAINT PK_PaymentStage PRIMARY KEY (Id));
