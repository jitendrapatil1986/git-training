CREATE NONCLUSTERED INDEX [IX_WarrantyPayment_PostingMonthPostingYear]
ON [dbo].[WarrantyPayments] ([PostingMonth],[PostingYear])
INCLUDE ([JobNumber],[Amount])
GO

CREATE NONCLUSTERED INDEX [IX_Jobs_CloseDate]
ON [dbo].[Jobs] ([CloseDate])
INCLUDE ([CommunityId])
GO