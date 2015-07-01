namespace Warranty.Core.Services
{
    using System;
    using Configurations;
    using Entities;

    public class ResolveObjectAccount : IResolveObjectAccount
    {
        public string ResolveLaborObjectAccount(Job job, ServiceCall serviceCall)
        {
            var isUnderOneYear = job.CloseDate.GetValueOrDefault().AddYears(1) >
                                 DateTime.Now.Date;

            return isUnderOneYear ? WarrantyConstants.UnderOneYearLaborCode : WarrantyConstants.OverOneYearLaborCode;
        }

        public string ResolveMaterialObjectAccount(Job job, ServiceCall serviceCall)
        {
            var isUnderOneYear = job.CloseDate.GetValueOrDefault().AddYears(1) >
                                 DateTime.Now.Date;

            return isUnderOneYear ? WarrantyConstants.UnderOneYearMaterialCode : WarrantyConstants.OverOneYearMaterialCode;
        }
    }
}