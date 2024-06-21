using System;
namespace Course.Service.Dtos.GroupDtos
{
	public class GroupGetDto
	{
        public int Id { get; set; }
        public string No { get; set; }

        public byte Limit { get; set; }

        public int StudentCount { get; set; }
    }
}

