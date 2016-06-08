using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using Warranty.Core.Features.MyTeam;

namespace Warranty.UI.Core.Automapper
{
    public class MappingProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<KeyValuePair<Guid, string>, SelectListItem>()
                .ForMember(m => m.Value, a => a.MapFrom(src => src.Key))
                .ForMember(m => m.Text, a => a.MapFrom(src => src.Value));

            CreateMap<MyTeamModel, SelectListItem>()
                .ForMember(m => m.Value, a => a.MapFrom(src => src.EmployeeId))
                .ForMember(m => m.Text, a => a.MapFrom(src => src.EmployeeName));
        }
    }
}