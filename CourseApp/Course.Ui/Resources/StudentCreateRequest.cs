using System;
using System.ComponentModel.DataAnnotations;

namespace Course.Ui.Resources
{
	public class StudentCreateRequest
	{
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public IFormFile FileName { get; set; }

        [Required]
        public int GroupId { get; set; }
    }
}

