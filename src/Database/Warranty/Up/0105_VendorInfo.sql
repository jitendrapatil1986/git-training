CREATE TABLE [dbo].[Vendors](
	[VendorId] [uniqueidentifier] NOT NULL,
	[Number] [varchar](50) NULL,
	[Name] [varchar](255) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [varchar](255) NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [varchar](255) NULL,
 CONSTRAINT [PK_Vendors] PRIMARY KEY CLUSTERED 
(
	[VendorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [ActiveData]
) ON [ActiveData]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Vendors] ADD  CONSTRAINT [DF_Vendors_Id]  DEFAULT (newsequentialid()) FOR [VendorId]
GO




CREATE TABLE [dbo].[VendorPhones](
	[VendorPhoneId] [uniqueidentifier] NOT NULL,
	[Number] [varchar](50) NULL,
	[Type] [varchar](50) NULL,
	[VendorId] [uniqueidentifier] NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [varchar](255) NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [varchar](255) NULL,
 CONSTRAINT [PK_VendorPhones] PRIMARY KEY CLUSTERED 
(
	[VendorPhoneId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [ActiveData]
) ON [ActiveData]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[VendorPhones] ADD  CONSTRAINT [DF_VendorPhones_Id]  DEFAULT (newsequentialid()) FOR [VendorPhoneId]
GO

ALTER TABLE [dbo].[VendorPhones]  WITH CHECK ADD  CONSTRAINT [FK_VendorPhones_VendorId] FOREIGN KEY([VendorId])
REFERENCES [dbo].[Vendors] ([VendorId])
GO






CREATE TABLE [dbo].[VendorEmails](
	[VendorEmailId] [uniqueidentifier] NOT NULL,
	[Email] [varchar](255) NULL,
	[VendorId] [uniqueidentifier] NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [varchar](255) NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [varchar](255) NULL,
 CONSTRAINT [PK_VendorEmails] PRIMARY KEY CLUSTERED 
(
	[VendorEmailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [ActiveData]
) ON [ActiveData]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[VendorEmails] ADD  CONSTRAINT [DF_VendorEmails_Id]  DEFAULT (newsequentialid()) FOR [VendorEmailId]
GO

ALTER TABLE [dbo].[VendorEmails]  WITH CHECK ADD  CONSTRAINT [FK_VendorEmails_VendorId] FOREIGN KEY([VendorId])
REFERENCES [dbo].[Vendors] ([VendorId])
GO




CREATE TABLE [dbo].[JobVendorCostCodes](
	[JobVendorCostCodeId] [uniqueidentifier] NOT NULL,
	[CostCode] [varchar](50) NULL,
	[CostCodeDescription] [varchar](255) NULL,
	[VendorId] [uniqueidentifier] NULL,
     [JobId] [uniqueidentifier] NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [varchar](255) NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [varchar](255) NULL,
 CONSTRAINT [PK_JobVendorCostCodes] PRIMARY KEY CLUSTERED 
(
	[JobVendorCostCodeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [ActiveData]
) ON [ActiveData]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[JobVendorCostCodes] ADD  CONSTRAINT [DF_JobVendorCostCode_Id]  DEFAULT (newsequentialid()) FOR [JobVendorCostCodeId]
GO

ALTER TABLE [dbo].[JobVendorCostCodes]  WITH CHECK ADD  CONSTRAINT [FK_JobVendorCostCodes_VendorId] FOREIGN KEY([VendorId])
REFERENCES [dbo].[Vendors] ([VendorId])
GO

ALTER TABLE [dbo].[JobVendorCostCodes]  WITH CHECK ADD  CONSTRAINT [FK_JobVendorCostCodes_JobId] FOREIGN KEY([JobId])
REFERENCES [dbo].[Jobs] ([JobId])
GO