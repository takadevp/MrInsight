using App.Common;
using System.IO;
using System.Text.Json;

namespace MrInsight;

public static class SampleGitLab
{
	private static readonly Lazy<JsonElement> Root = new Lazy<JsonElement>(Load);

	public static JsonElement Find(string path)
	{
		var separatorIndex = path.IndexOf('?');
		var key = separatorIndex >= 0 ? path.Substring(0, separatorIndex) : path;
		if (!Root.Value.TryGetProperty(key, out var value))
		{
			throw new GitLabStatusException($"見本のデータに {key} がありません。", new { key });
		}

		return value;
	}

	private static JsonElement Load()
	{
		var path = Path.Combine(AppEnvironment.ProjectPath, "sample", "gitlab.json");
		if (!File.Exists(path))
		{
			throw new AppException("見本のデータがありません。", new { path }, 400);
		}

		using var document = JsonDocument.Parse(File.ReadAllText(path));
		return document.RootElement.Clone();
	}
}
