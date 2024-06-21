using System;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;


namespace Course.Core.Entities
{
	public class Group:AuditEntity
	{
       

        public string No { get; set; }

        public byte Limit { get; set; }

        public List<Student> Students { get; set; }
     
    }
}

