using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MrInsight;

public static class CrossTableView
{
	public static readonly DependencyProperty TableProperty = DependencyProperty.RegisterAttached(
		"Table",
		typeof(CrossTable),
		typeof(CrossTableView),
		new PropertyMetadata(null, OnTableChanged));

	public static CrossTable? GetTable(DependencyObject element)
	{
		return (CrossTable?)element.GetValue(TableProperty);
	}

	public static void SetTable(DependencyObject element, CrossTable? value)
	{
		element.SetValue(TableProperty, value);
	}

	private static void OnTableChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
	{
		if (element is not DataGrid dataGrid)
		{
			return;
		}

		dataGrid.ItemsSource = null;
		dataGrid.Columns.Clear();
		if (e.NewValue is not CrossTable table)
		{
			return;
		}

		// 見出しには . や / を含むユーザー名が入るので、列名でなく位置でバインドする
		for (var index = 0; index < table.HeaderList.Count; index++)
		{
			dataGrid.Columns.Add(new DataGridTextColumn
			{
				Header = table.HeaderList[index],
				Binding = new Binding($"[{index}]") { Mode = BindingMode.OneWay },
			});
		}

		dataGrid.ItemsSource = table.RowList;
	}
}
