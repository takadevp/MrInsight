using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;

namespace MrInsight;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
		if (AppEnvironment.IsSample)
		{
			Title = "MrInsight 見本";
		}

		AppLog.LineList.CollectionChanged += OnLogLineListChanged;
	}

	private void OnLogLineListChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		ScrollLogToEnd();
	}

	private void OnLogListBoxIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		ScrollLogToEnd();
	}

	private void OnCopyLogCanExecute(object sender, CanExecuteRoutedEventArgs e)
	{
		e.CanExecute = LogListBox.SelectedItems.Count > 0;
	}

	private void OnCopyLogExecuted(object sender, ExecutedRoutedEventArgs e)
	{
		var indexList = new List<int>();
		foreach (var item in LogListBox.SelectedItems)
		{
			indexList.Add(AppLog.LineList.IndexOf((string)item));
		}

		indexList.Sort();
		var lineList = new List<string>();
		foreach (var index in indexList)
		{
			lineList.Add(AppLog.LineList[index]);
		}

		Clipboard.SetText(string.Join(Environment.NewLine, lineList));
	}

	private void OnClearLogClick(object sender, RoutedEventArgs e)
	{
		AppLog.LineList.Clear();
	}

	private void ScrollLogToEnd()
	{
		if (!LogListBox.IsVisible || AppLog.LineList.Count <= 0)
		{
			return;
		}

		LogListBox.ScrollIntoView(AppLog.LineList[^1]);
	}
}
