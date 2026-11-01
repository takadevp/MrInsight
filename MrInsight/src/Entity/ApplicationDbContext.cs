using App.Common;
using Microsoft.EntityFrameworkCore;

namespace MrInsight;

public abstract class ApplicationDbContext(string connectionString) : AbstractDbContext
{
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		base.OnConfiguring(optionsBuilder);
		optionsBuilder.UseSqlite(connectionString);
	}
}
