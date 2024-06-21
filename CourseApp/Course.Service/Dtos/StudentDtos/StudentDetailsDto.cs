using System;
namespace Course.Service.Dtos.StudentDtos
{
	public class StudentDetailsDto
	{
        public string FullName { get; set; }

        public string Email { get; set; }

        public DateTime Birthdate { get; set; }

        public string GroupName { get; set; }

        public int GroupId { get; set; }
    }
}

