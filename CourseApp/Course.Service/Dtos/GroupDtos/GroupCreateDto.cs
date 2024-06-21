using System;

using System.ComponentModel.DataAnnotations.Schema;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Course.Service.Dtos.GroupDtos
{
	public class GroupCreateDto
	{
        public string No { get; set; }

        public byte Limit { get; set; }
        
    
    }
    public class CourseCreateDtoValidator : AbstractValidator<GroupCreateDto>
    {
        public CourseCreateDtoValidator()
        {
            RuleFor(x => x.No).NotEmpty().MaximumLength(5).MinimumLength(4);

            RuleFor(x => (int)x.Limit).NotNull().InclusiveBetween(5, 20);

           
        }
    }
}

