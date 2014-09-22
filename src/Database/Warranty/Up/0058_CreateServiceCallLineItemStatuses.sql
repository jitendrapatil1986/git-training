CREATE TABLE [lookups].[ServiceCallLineItemStatuses](
	[ServiceCallLineItemStatusId] [tinyint] NOT NULL,  --Not set to Identity b/c we will manage lookup id's w/i the application.
	[ServiceCallLineItemStatus] [varchar](20) NULL,
 CONSTRAINT [PK_ServiceCallLineItemStatuses] PRIMARY KEY CLUSTERED 
(
	[ServiceCallLineItemStatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [ActiveData]
) ON [ActiveData]