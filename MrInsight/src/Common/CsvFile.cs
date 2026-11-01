using App.Common;
using Microsoft.Win32;
using System.Globalization;
using System.IO;
using System.Text;

namespace MrInsight;

public static class CsvFile
{
	public static void Save(string fileName, List<string> headerList, List<List<object?>> rowList)
	{
		var dialog = new SaveFileDialog
		{
			FileName = $"{fileName}_{DateTime.Now:yyyyMMdd}.csv",
			DefaultExt = ".csv",
			Filter = "CSV ファイル (*.csv)|*.csv",
		};
		if (dialog.ShowDialog() != true)
		{
			return;
		}

		var builder = new StringBuilder();
		var headerCellList = new List<string>();
		foreach (var header in headerList)
		{
			headerCellList.Add(Escape(header));
		}

		builder.Append(string.Join(",", headerCellList));
		builder.Append("\r\n");
		foreach (var row in rowList)
		{
			var cellList = new List<string>();
			foreach (var value in row)
			{
				cellList.Add(Escape(Format(value)));
			}

			builder.Append(string.Join(",", cellList));
			builder.Append("\r\n");
		}

		try
		{
			File.WriteAllText(dialog.FileName, builder.ToString(), new UTF8Encoding(true));
		}
		catch (IOException exception)
		{
			throw new AppException("CSV ファイルに書き込めませんでした。Excel で開いたままなら閉じてください。", new { path = dialog.FileName, exception.Message }, 400);
		}
		catch (UnauthorizedAccessException exception)
		{
			throw new AppException("CSV ファイルを書き込む権限がありません。", new { path = dialog.FileName, exception.Message }, 400);
		}
	}

	private static string Format(object? value)
	{
		if (value == null)
		{
			return string.Empty;
		}

		if (value is DateTime dateTime)
		{
			return dateTime.ToString("yyyy/MM/dd HH:mm", CultureInfo.InvariantCulture);
		}

		return Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
	}

	private static string Escape(string text)
	{
		if (text.IndexOfAny([',', '"', '\r', '\n']) < 0)
		{
			return text;
		}

		return $"\"{text.Replace("\"", "\"\"")}\"";
	}
}
