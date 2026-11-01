using App.Common;
using System.IO;
using System.Text.Json;

namespace MrInsight;

public class AppSettings
{
	public string GitLabUrl { get; set; } = string.Empty;

	public string GitLabAccessToken { get; set; } = string.Empty;

	public bool UseSample { get; set; }

	public static AppSettings Load()
	{
		var path = Path.Combine(AppEnvironment.ProjectPath, "settings.json");
		if (!File.Exists(path))
		{
			throw new AppException("settings.json がありません。settings.example.json を写して作ってください。", new { path }, 400);
		}

		var json = File.ReadAllText(path);
		var settings = JsonSerializer.Deserialize<AppSettings>(json);
		if (settings != null && settings.UseSample)
		{
			return settings;
		}

		if (settings == null || settings.GitLabUrl.Length <= 0 || settings.GitLabAccessToken.Length <= 0)
		{
			throw new AppException("settings.json に GitLabUrl と GitLabAccessToken を書いてください。", new { path }, 400);
		}

		return settings;
	}

	public static bool ReadUseSample()
	{
		var path = Path.Combine(AppEnvironment.ProjectPath, "settings.json");
		if (!File.Exists(path))
		{
			return false;
		}

		var settings = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(path));
		return settings != null && settings.UseSample;
	}
}
