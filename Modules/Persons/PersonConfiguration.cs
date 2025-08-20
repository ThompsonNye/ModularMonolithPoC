using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ModularMonolithPoC.Persons;

internal sealed class PersonConfiguration : IEntityTypeConfiguration<Person>
{
	public void Configure(EntityTypeBuilder<Person> builder)
	{
		builder.Property(x => x.Id)
			.IsRequired();

		builder.HasKey(x => x.Id);

		builder.Property(x => x.Name)
			.IsRequired()
			.HasMaxLength(DbModelConstants.NAME_MAX_LENGTH);
	}
}
