namespace MrInsight;

public sealed class CrossTableCell
{
	public List<string> RowKeyList { get; } = [];

	public string ColumnKey { get; set; } = string.Empty;

	public int Value { get; set; }

	public double? Ratio { get; set; }
}
