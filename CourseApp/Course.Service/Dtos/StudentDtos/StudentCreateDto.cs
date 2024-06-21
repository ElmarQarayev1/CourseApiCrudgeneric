using System;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Course.Service.Dtos.StudentDtos
{
	public class StudentCreateDto
	{
        public string FullName { get; set; }

        public string Email { get; set; }

        public DateTime Birthdate { get; set; }

        public int GroupId { get; set; }

        public IFormFile File { get; set; }
    }
    public class StudentCreateDtoValidator : AbstractValidator<StudentCreateDto>
    {
        public StudentCreateDtoValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(35).MinimumLength(6);

            RuleFor(x => x.Email).NotNull().EmailAddress();

            RuleFor(x => x.Birthdate).NotEmpty().Must(BeAtLeast14YearsOld);

            RuleFor(x => x.GroupId).NotNull().GreaterThan(0);

            RuleFor(x => x.File)
              .Must(file => file == null || file.Length <= 2 * 1024 * 1024)
              .WithMessage("File must be less or equal than 2MB")
              .Must(file => file == null || new[] { "image/png", "image/jpeg" }.Contains(file.ContentType))
              .WithMessage("File type must be png, jpeg, or jpg");

        }
        private bool BeAtLeast14YearsOld(DateTime birthdate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthdate.Year;

            if (birthdate.Date > today.AddYears(-age))
            {
                age--;
            }
            return age >= 14;
        }
    }

}

