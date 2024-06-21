using System;
using AutoMapper;
using Course.Core.Entities;
using Course.Service.Dtos.GroupDtos;
using Course.Service.Dtos.StudentDtos;
using Microsoft.AspNetCore.Http;

namespace Course.Service.Profiles
{
	public class MapProfile:Profile
	{
        private readonly IHttpContextAccessor _context;

        public MapProfile(IHttpContextAccessor httpContextAccessor)
		{
            _context = httpContextAccessor;

            var uriBuilder = new UriBuilder(_context.HttpContext.Request.Scheme, _context.HttpContext.Request.Host.Host, _context.HttpContext.Request.Host.Port ?? -1);

            if (uriBuilder.Uri.IsDefaultPort)
            {
                uriBuilder.Port = -1;
            }
            string baseUrl = uriBuilder.Uri.AbsoluteUri;

            CreateMap<Group, GroupGetDto>()
            .ForMember(dest => dest.StudentCount, s => s.MapFrom(s => s.Students.Count));
            CreateMap<GroupCreateDto, Group>();
            CreateMap<Group, GroupDetailsDto>();
            CreateMap<Group, GroupGetDto>();

            CreateMap<Student, StudentDetailsDto>()
                .ForMember(dest => dest.GroupName, s => s.MapFrom(s => s.Group.No));
            CreateMap<Student, StudentGetDto>()
              .ForMember(dest => dest.Age, s => s.MapFrom(s => DateTime.Now.Year - s.BirthDate.Year))
              .ForMember(dest => dest.ImageUrl, s => s.MapFrom(s => baseUrl + "uploads/students/" + s.FileName));

        }
    }
}

