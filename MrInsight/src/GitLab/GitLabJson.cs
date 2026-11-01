using System.Text.Json;

namespace MrInsight;

public static class GitLabJson
{
	public static JsonElement? GetObject(JsonElement element, string name)
	{
		if (element.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.Object)
		{
			return value;
		}

		return null;
	}

	public static List<JsonElement> GetArray(JsonElement element, string name)
	{
		var elementList = new List<JsonElement>();
		if (element.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.Array)
		{
			foreach (var item in value.EnumerateArray())
			{
				elementList.Add(item);
			}
		}

		return elementList;
	}

	public static string? GetString(JsonElement? element, string name)
	{
		if (element.HasValue && element.Value.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String)
		{
			return value.GetString();
		}

		return null;
	}

	public static int? GetInt(JsonElement? element, string name)
	{
		if (element.HasValue && element.Value.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.Number)
		{
			return value.GetInt32();
		}

		return null;
	}

	public static bool? GetBool(JsonElement element, string name)
	{
		if (element.TryGetProperty(name, out var value) && (value.ValueKind == JsonValueKind.True || value.ValueKind == JsonValueKind.False))
		{
			return value.GetBoolean();
		}

		return null;
	}

	public static DateTime? GetDate(JsonElement element, string name)
	{
		if (element.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String)
		{
			var offset = value.GetDateTimeOffset();
			return offset.LocalDateTime;
		}

		return null;
	}
}
