CREATE TABLE [dbo].[Backcharges](
	[BackchargeId] [uniqueidentifier] NOT NULL,
	[BackchargeVendorNumber] [varchar](8) NULL,
	[BackchargeReason] [varchar](max) NULL,
	[PersonNotified] [varchar](255) NULL,
	[PersonNotifiedPhoneNumber] [varchar](255) NULL,
	[PersonNotifiedDate] [datetime2](7) NULL,
	[BackchargeResponseFromVendor] [varchar](max) NULL

 CONSTRAINT [PK_Backcharges] PRIMARY KEY CLUSTERED 
(
	[BackchargeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [ActiveData]
) ON [ActiveData]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Backcharges] ADD  CONSTRAINT [DF_Backcharges_BackchargeId]  DEFAULT (newsequentialid()) FOR [BackchargeId]
GO


