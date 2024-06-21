using System;
using System.ComponentModel.DataAnnotations;

namespace Course.Ui.Resources
{
	public class GroupCreateRequest
	{
        [MaxLength(5)]
        [MinLength(4)]
        public string No { get; set; }
        [Range(5, 18)]
        public byte Limit { get; set; }
    }
}

