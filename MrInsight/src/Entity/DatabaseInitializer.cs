using App.Common;
using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace MrInsight;

public static class DatabaseInitializer
{
	public static void Initialize()
	{
		var assembly = Assembly.GetExecutingAssembly();
		var nameList = new List<string>();
		foreach (var name in assembly.GetManifestResourceNames())
		{
			if (name.EndsWith(".sql", StringComparison.Ordinal))
			{
				nameList.Add(name);
			}
		}

		nameList.Sort(StringComparer.Ordinal);

		using (var context = ApplicationGlobalDbContext.Create())
		{
			context.Database.ExecuteSqlRaw("PRAGMA journal_mode=WAL;");
		}

		using var transaction = DatabaseTransaction.Begin();
		foreach (var name in nameList)
		{
			using var stream = assembly.GetManifestResourceStream(name) ?? throw new AppException("テーブル定義が読めません。", new { name });
			using var reader = new StreamReader(stream);
			var sql = reader.ReadToEnd();
			DbContextScope.GlobalContext.Database.ExecuteSqlRaw(sql);
		}

		transaction.Commit();
	}
}
