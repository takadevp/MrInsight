namespace MrInsight;

public sealed class CrossTable
{
	private const string TotalHeader = "合計";

	private const string RowKeySeparator = "\u001f";

	public static CrossTable Empty => new CrossTable();

	public List<string> HeaderList { get; } = [];

	public List<string[]> RowList { get; } = [];

	public static CrossTable Create(List<string> rowHeaderList, List<CrossTableCell> cellList)
	{
		var rowKeyList = new List<string>();
		var rowCellDictionary = new Dictionary<string, Dictionary<string, CrossTableCell>>();
		var rowLabelDictionary = new Dictionary<string, List<string>>();
		var rowTotalDictionary = new Dictionary<string, int>();
		var columnTotalDictionary = new Dictionary<string, int>();
		var grandTotal = 0;
		var hasRatio = false;
		foreach (var cell in cellList)
		{
			var rowKey = string.Join(RowKeySeparator, cell.RowKeyList);
			if (!rowCellDictionary.TryGetValue(rowKey, out var columnCellDictionary))
			{
				columnCellDictionary = new Dictionary<string, CrossTableCell>();
				rowCellDictionary[rowKey] = columnCellDictionary;
				rowLabelDictionary[rowKey] = cell.RowKeyList;
				rowTotalDictionary[rowKey] = 0;
				rowKeyList.Add(rowKey);
			}

			columnCellDictionary[cell.ColumnKey] = cell;
			rowTotalDictionary[rowKey] += cell.Value;
			columnTotalDictionary.TryGetValue(cell.ColumnKey, out var columnTotal);
			columnTotalDictionary[cell.ColumnKey] = columnTotal + cell.Value;
			grandTotal += cell.Value;
			if (cell.Ratio != null)
			{
				hasRatio = true;
			}
		}

		var columnList = new List<KeyValuePair<string, int>>(columnTotalDictionary);
		columnList.Sort(CompareColumn);

		var table = new CrossTable();
		table.HeaderList.AddRange(rowHeaderList);
		foreach (var column in columnList)
		{
			table.HeaderList.Add(column.Key);
		}

		table.HeaderList.Add(TotalHeader);

		foreach (var rowKey in rowKeyList)
		{
			var columnCellDictionary = rowCellDictionary[rowKey];
			var row = new List<string>(rowLabelDictionary[rowKey]);
			foreach (var column in columnList)
			{
				if (columnCellDictionary.TryGetValue(column.Key, out var cell))
				{
					row.Add(Format(cell.Value, cell.Ratio));
				}
				else
				{
					row.Add(string.Empty);
				}
			}

			row.Add(rowTotalDictionary[rowKey].ToString());
			table.RowList.Add(row.ToArray());
		}

		if (rowKeyList.Count <= 0)
		{
			return table;
		}

		var totalRow = new List<string> { TotalHeader };
		for (var index = 1; index < rowHeaderList.Count; index++)
		{
			totalRow.Add(string.Empty);
		}

		foreach (var column in columnList)
		{
			double? ratio = null;
			if (hasRatio && grandTotal > 0)
			{
				ratio = Math.Round(100.0 * column.Value / grandTotal, 1);
			}

			totalRow.Add(Format(column.Value, ratio));
		}

		totalRow.Add(grandTotal.ToString());
		table.RowList.Add(totalRow.ToArray());
		return table;
	}

	private static int CompareColumn(KeyValuePair<string, int> left, KeyValuePair<string, int> right)
	{
		if (left.Value != right.Value)
		{
			return right.Value.CompareTo(left.Value);
		}

		return string.CompareOrdinal(left.Key, right.Key);
	}

	private static string Format(int value, double? ratio)
	{
		if (ratio == null)
		{
			return value.ToString();
		}

		return $"{value} ({ratio:0.0}%)";
	}
}
