using System;
using Course.Core.Entities;
using Course.Data.Repostories.Interfaces;

namespace Course.Data.Repostories.Implementations
{
	public class StudentRepository: Repository<Student>, IStudentRepository
    {
		public StudentRepository(AppDbContext context):base(context)
		{
		}
	}
}

