using System.IO;

namespace MrInsight;

public static class AppEnvironment
{
	public static string ProjectPath => FindProjectPath();

	public static bool IsSample { get; } = AppSettings.ReadUseSample();

	public static string DatabasePath
	{
		get
		{
			var directory = Path.Combine(ProjectPath, "database");
			Directory.CreateDirectory(directory);
			return Path.Combine(directory, IsSample ? "MrInsight-sample.db" : "MrInsight.db");
		}
	}

	public static string ConnectionString => $"Data Source={DatabasePath}";

	private static string FindProjectPath()
	{
		var directory = new DirectoryInfo(AppContext.BaseDirectory);
		while (directory != null)
		{
			if (File.Exists(Path.Combine(directory.FullName, "MrInsight.csproj")))
			{
				return directory.FullName;
			}

			directory = directory.Parent;
		}

		return AppContext.BaseDirectory;
	}
}
