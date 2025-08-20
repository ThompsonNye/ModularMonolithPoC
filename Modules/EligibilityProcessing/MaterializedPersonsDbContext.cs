using Microsoft.EntityFrameworkCore;

namespace ModularMonolithPoC.EligibilityProcessing;

internal sealed class MaterializedPersonsDbContext(DbContextOptions<MaterializedPersonsDbContext> options) : DbContext(options)
{
	public DbSet<Person> Persons => Set<Person>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(IEligibilityProcessingMarker).Assembly);

		modelBuilder.HasDefaultSchema("eligibility_processing");
	}
}
