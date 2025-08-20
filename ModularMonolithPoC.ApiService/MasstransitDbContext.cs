using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ModularMonolithPoC.ApiService;

public sealed class MasstransitDbContext(DbContextOptions<MasstransitDbContext> options) : DbContext(options)
{
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.AddTransactionalOutboxEntities();
	}
}