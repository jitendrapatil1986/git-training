﻿using System;
using System.Collections.Generic;
using System.Linq;
using NPoco;
using Warranty.Core.Entities;
using Warranty.Core.Services;

namespace Warranty.Core.Features.Vendor
{
    public class UpdateVendorDataCommandHandler : ICommandHandler<UpdateVendorDataCommand>
    {
        private readonly IJobService _jobService;
        private readonly IAccountingService _accountingService;
        private readonly IDatabase _database;

        public UpdateVendorDataCommandHandler(IJobService jobService, IAccountingService accountingService, IDatabase database)
        {
            _jobService = jobService;
            _accountingService = accountingService;
            _database = database;
        }

        public void Handle(UpdateVendorDataCommand message)
        {
            var job = _jobService.GetJobById(message.JobId);

            var vendors = _accountingService.Execute(x => x.Get.VendorForJob(
                new
                {
                    jobNumber = job.JobNumber
                }));

            IEnumerable<VendorForJob> vendorsResult = vendors.Details.ToObject<List<VendorForJob>>();
            var vendorNumbers = vendorsResult.Select(x => x.VendorNumber).Distinct().ToList();
            foreach (var vendorNumber in vendorNumbers)
            {
                var vendorInfo = vendorsResult.Where(x => x.VendorNumber == vendorNumber);
                var vendorForJob = vendorInfo.First();
                var vendor = _database.SingleOrDefault<Entities.Vendor>("Select * FROM Vendors where Number = @0",
                                                               vendorForJob.VendorNumber);
                if (vendor == null)
                {
                    vendor = new Entities.Vendor
                    {
                        Name = vendorForJob.VendorName,
                        Number = vendorForJob.VendorNumber
                    };

                    _database.Insert(vendor);
                }
                else
                {
                    vendor.Name = vendorForJob.VendorName;
                    _database.Update(vendor);
                }
                UpdateVendorInfo(vendorInfo, vendor);
                PersistVendorCostCodesForJob(vendorInfo, vendor, job.JobId);
            }
        }

        private void UpdateVendorInfo(IEnumerable<VendorForJob> vendorInfo, Entities.Vendor vendor)
        {
            _database.Execute("DELETE FROM VendorPhones WHERE VendorId=@0", vendor.VendorId);
            _database.Execute("DELETE FROM VendorEmails WHERE VendorId=@0", vendor.VendorId);

            vendorInfo.Select(x => new { x.VendorPhone, x.PhoneType }).ToList().ForEach(vp =>
            {
                var vendorPhone = new VendorPhone
                {
                    VendorId = vendor.VendorId,
                    Number = vp.VendorPhone,
                    Type = vp.PhoneType
                };
                _database.Insert(vendorPhone);
            });

            vendorInfo.Select(x => new { x.VendorEmail }).ToList().ForEach(e =>
            {
                var vendorEmail = new VendorEmail()
                {
                    VendorId = vendor.VendorId,
                    Email = e.VendorEmail,
                };
                _database.Insert(vendorEmail);
            });
        }

        private void PersistVendorCostCodesForJob(IEnumerable<VendorForJob> vendorInfo, Entities.Vendor vendor, Guid jobId)
        {
            vendorInfo.Select(x => new { x.CostCode, x.Description }).ToList().ForEach(jvcc =>
            {
                var jobVendorCostCode = new JobVendorCostCode()
                {
                    VendorId = vendor.VendorId,
                    CostCode = jvcc.CostCode,
                    CostCodeDescription = jvcc.Description,
                    JobId = jobId
                };
                _database.Insert(jobVendorCostCode);
            });
        }

        public class VendorForJob
        {
            public string VendorName { get; set; }
            public string VendorPhone { get; set; }
            public string Description { get; set; }
            public string CostCode { get; set; }
            public string PhoneType { get; set; }
            public string VendorEmail { get; set; }
            public string VendorNumber { get; set; }
            public string HeldStatus { get; set; }
        }
    }
}