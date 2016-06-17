if dbo.IndexExists('JobVendorCostCodes', 'IX_JobVendorCostCodes_JobIdCostCodeCostCodeDescriptionVendorId') = 1
begin
    drop index dbo.JobVendorCostCodes.IX_JobVendorCostCodes_JobIdCostCodeCostCodeDescriptionVendorId;
end
go

CREATE NONCLUSTERED INDEX [IX_JobVendorCostCodes_JobIdCostCodeCostCodeDescriptionVendorId]
ON [dbo].[JobVendorCostCodes] ([JobId])
INCLUDE ([CostCode],[CostCodeDescription],[VendorId])