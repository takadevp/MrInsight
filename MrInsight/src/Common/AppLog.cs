using App.Common;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Windows;

namespace MrInsight;

public static class AppLog
{
	private const int MaxLineCount = 2000;

	private static readonly object FileLock = new();

	private static readonly JsonSerializerOptions ContextJsonOptions = new()
	{
		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
	};

	public static ObservableCollection<string> LineList { get; } = [];

	public static string FilePath
	{
		get
		{
			var directory = Path.Combine(AppEnvironment.ProjectPath, "log");
			Directory.CreateDirectory(directory);
			return Path.Combine(directory, "MrInsight.log");
		}
	}

	public static void Write(string message)
	{
		var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} {message}";
		WriteFile(line);

		var dispatcher = Application.Current?.Dispatcher;
		if (dispatcher == null || dispatcher.CheckAccess())
		{
			AddLine(line);
			return;
		}

		dispatcher.BeginInvoke(new Action<string>(AddLine), line);
	}

	public static void Write(Exception exception)
	{
		if (exception is AppException appException && appException.Context != null)
		{
			var context = JsonSerializer.Serialize(appException.Context, ContextJsonOptions);
			Write($"{exception}{Environment.NewLine}context: {context}");
			return;
		}

		Write(exception.ToString());
	}

	private static void WriteFile(string line)
	{
		lock (FileLock)
		{
			File.AppendAllText(FilePath, line + Environment.NewLine);
		}
	}

	private static void AddLine(string line)
	{
		LineList.Add(line);
		while (LineList.Count > MaxLineCount)
		{
			LineList.RemoveAt(0);
		}
	}
}
