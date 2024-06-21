using System;
namespace Course.Service.Dtos.StudentDtos
{
	public class StudentGetDto
	{
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string GroupName { get; set; }

        public string ImageUrl { get; set; }

        public int Age { get; set; }

    }
}

