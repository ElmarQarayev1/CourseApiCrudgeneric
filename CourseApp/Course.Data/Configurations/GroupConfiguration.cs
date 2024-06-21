using System;
using Course.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Course.Data.Configurations
{
	public class GroupConfiguration:IEntityTypeConfiguration<Group>
    {
		
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.Property(x => x.No).HasMaxLength(5).IsRequired(true);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Limit).IsRequired(true);
        }
    }
}

