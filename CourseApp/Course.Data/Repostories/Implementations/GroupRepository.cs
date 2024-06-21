using System;
using Course.Core.Entities;
using Course.Data.Repostories.Interfaces;

namespace Course.Data.Repostories.Implementations
{
	public class GroupRepository: Repository<Group>, IGroupRepository
    {
		public GroupRepository(AppDbContext context):base(context)
		{

		}
	}
}

