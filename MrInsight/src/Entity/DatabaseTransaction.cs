using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace MrInsight;

public sealed class DatabaseTransaction : IDisposable
{
	private readonly ApplicationGlobalDbContext _context;

	private bool _isCommitted;

	private DatabaseTransaction(ApplicationGlobalDbContext context)
	{
		_context = context;
	}

	public static DatabaseTransaction Begin()
	{
		var context = ApplicationGlobalDbContext.Create();
		context.Database.OpenConnection();
		var connection = (SqliteConnection)context.Database.GetDbConnection();
		var sqliteTransaction = connection.BeginTransaction(deferred: true);
		context.Database.UseTransaction(sqliteTransaction);
		DbContextScope.GlobalContext = context;
		return new DatabaseTransaction(context);
	}

	public void Commit()
	{
		_context.Database.CommitTransaction();
		_isCommitted = true;
	}

	public void Dispose()
	{
		if (!_isCommitted && _context.Database.CurrentTransaction != null)
		{
			_context.Database.RollbackTransaction();
		}

		DbContextScope.Clear();
		_context.Dispose();
	}
}
