using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ModularMonolithPoC.Persons;

internal sealed class PersonsDbContext(DbContextOptions<PersonsDbContext> options) : DbContext(options)
{
	public DbSet<Person> Persons => Set<Person>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(IPersonsMarker).Assembly);

		modelBuilder.HasDefaultSchema("persons");

		AddMassTransit(modelBuilder);
	}

	private static void AddMassTransit(ModelBuilder modelBuilder)
		=> modelBuilder.AddTransactionalOutboxEntities();
}
