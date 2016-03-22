using AutoMapper;
using System.Linq;
using TIPS.Commands.Responses;
using Warranty.Core.Entities;

namespace Warranty.Server
{
    public class MappingProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<HomeBuyerDetailsResponse, HomeOwner>()
                .ForMember(m => m.HomeOwnerId, a => a.Ignore())
                .ForMember(m => m.CreatedBy, a => a.Ignore())
                .ForMember(m => m.CreatedDate, a => a.Ignore())
                .ForMember(m => m.UpdatedDate, a => a.Ignore())
                .ForMember(m => m.UpdatedBy, a => a.Ignore())
                .ForMember(m => m.JobId, a => a.Ignore())
                .ForMember(m => m.HomeOwnerNumber, a => a.Ignore())
                .ForMember(m => m.WorkPhone1, a => a.Ignore())
                .ForMember(m => m.WorkPhone2, a => a.Ignore())
                .ForMember(m => m.OtherPhone, a => a.Ignore())
                .ForMember(m => m.HomeOwnerName, a => a.MapFrom(src => GetHomeOwnerName(src)))
                .ForMember(m => m.HomePhone, a => a.MapFrom(src => GetPrimaryPhone(src)))
                .ForMember(m => m.EmailAddress, a => a.MapFrom(src => GetPrimaryEmailAddress(src)));

            CreateMap<JobSaleDetailsResponse, Job>()
                .ForMember(m => m.JobId, a => a.Ignore())
                .ForMember(m => m.CreatedBy, a => a.Ignore())
                .ForMember(m => m.CreatedDate, a => a.Ignore())
                .ForMember(m => m.UpdatedDate, a => a.Ignore())
                .ForMember(m => m.UpdatedBy, a => a.Ignore())
                .ForMember(m => m.PlanTypeDescription, a => a.Ignore())
                .ForMember(m => m.CommunityId, a => a.Ignore())
                .ForMember(m => m.CurrentHomeOwnerId, a => a.Ignore())
                .ForMember(m => m.WarrantyExpirationDate, a => a.Ignore())
                .ForMember(m => m.DoNotContact, a => a.Ignore())
                .ForMember(m => m.BuilderEmployeeId, a => a.Ignore())
                .ForMember(m => m.SalesConsultantEmployeeId, a => a.Ignore())
                .ForMember(m => m.LegalDescription, a => a.MapFrom(src => GetLegalDescription(src)))
                .ForMember(m => m.AddressLine, a => a.MapFrom(src => src.AddressLine1))
                .ForMember(m => m.City, a => a.MapFrom(src => src.AddressCity))
                .ForMember(m => m.StateCode, a => a.MapFrom(src => src.AddressStateAbbreviation))
                .ForMember(m => m.PostalCode, a => a.MapFrom(src => src.AddressZipCode))
                .ForMember(m => m.PlanType, a => a.MapFrom(src => src.JobType))
                .ForMember(m => m.JdeIdentifier, a => a.MapFrom(src => src.JobNumber));
        }

        private string GetLegalDescription(JobSaleDetailsResponse src)
        {
            if (string.IsNullOrWhiteSpace(src.Lot) && string.IsNullOrWhiteSpace(src.Block) &&
                string.IsNullOrWhiteSpace(src.Section) && string.IsNullOrWhiteSpace(src.Phase))
                return null;

            return string.Format("{0}/{1}/{2}/{3}", src.Lot, src.Block, src.Section, src.Phase);
        }

        private string GetPrimaryEmailAddress(HomeBuyerDetailsResponse src)
        {
            if (src.EmailAddresses == null || !src.EmailAddresses.Any())
                return null;

            var primaryEmails = src.EmailAddresses.Where(x => x.IsPrimary).ToList();
            if (primaryEmails.Any())
                return primaryEmails.First().Address;

            return null;
        }

        private string GetPrimaryPhone(HomeBuyerDetailsResponse src)
        {
            if (src.PhoneNumbers == null || !src.PhoneNumbers.Any())
                return null;

            var primaryPhone = src.PhoneNumbers.Where(x => x.IsPrimary).ToList();
            if (primaryPhone.Any())
                return primaryPhone.First().Number;

            return null;
        }

        private string GetHomeOwnerName(HomeBuyerDetailsResponse src)
        {
            if (string.IsNullOrWhiteSpace(src.FirstName) && string.IsNullOrWhiteSpace(src.LastName))
                return null;

            return string.Format("{0}, {1}", src.LastName, src.FirstName);
        }
    }
}