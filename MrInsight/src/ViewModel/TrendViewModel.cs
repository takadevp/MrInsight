using App.Common;

namespace MrInsight;

/// <summary>
/// プロジェクトを選んで、月ごとに付いたバッジの件数と割合と、月ごとの作成者のマージリクエストの件数を出す。
/// 指摘が減ってきたかを見る
/// </summary>
public sealed class TrendViewModel : ViewModelBase
{
	private string _errorMessage = string.Empty;

	/// <summary>
	/// エラーの文面。無ければ空
	/// </summary>
	public string ErrorMessage
	{
		get => _errorMessage;
		private set => SetProperty(ref _errorMessage, value);
	}

	private RelayCommand? _refreshCommand;

	/// <summary>
	/// 再読み込みを実行するコマンド
	/// </summary>
	public RelayCommand RefreshCommand => _refreshCommand ??= new RelayCommand(ExecuteRefresh, CanExecuteRefresh);

	private List<Project> _projectItemList = [];

	private Project? _projectSelectedItem;

	/// <summary>
	/// プロジェクトの行
	/// </summary>
	public List<Project> ProjectItemList
	{
		get => _projectItemList;
		private set => SetProperty(ref _projectItemList, value);
	}

	/// <summary>
	/// プロジェクトで選んだ行
	/// </summary>
	public Project? ProjectSelectedItem
	{
		get => _projectSelectedItem;
		set
		{
			if (SetProperty(ref _projectSelectedItem, value))
			{
				LoadMonthlyBadgeCountTable();
				LoadMonthlyCountTable();
			}
		}
	}

	private CrossTable _monthlyBadgeCountTableSource = CrossTable.Empty;

	/// <summary>
	/// 月別バッジ件数表に出す、並べ直した表
	/// </summary>
	public CrossTable MonthlyBadgeCountTableSource
	{
		get => _monthlyBadgeCountTableSource;
		private set => SetProperty(ref _monthlyBadgeCountTableSource, value);
	}

	private CrossTable _monthlyCountTableSource = CrossTable.Empty;

	/// <summary>
	/// 月別件数表に出す、並べ直した表
	/// </summary>
	public CrossTable MonthlyCountTableSource
	{
		get => _monthlyCountTableSource;
		private set => SetProperty(ref _monthlyCountTableSource, value);
	}

	/// <summary>
	/// 画面を開いたときに、要求の無いパーツを読み込む
	/// </summary>
	public TrendViewModel()
	{
		LoadProject();
	}

	/// <summary>
	/// 再読み込みを押せるか
	/// </summary>
	private bool CanExecuteRefresh()
	{
		if (IsRunning)
		{
			return false;
		}

		return true;
	}

	/// <summary>
	/// 再読み込みを実行する
	/// </summary>
	private void ExecuteRefresh()
	{
		LoadProject();
	}

	/// <summary>
	/// プロジェクトを読み込む
	/// </summary>
	private void LoadProject()
	{
		try
		{
			var response = MergeRequestService.GetProjectList();
			// 表は新しい一覧に置き換わると選択を外すので、置き換える前に選んでいた行の主キーを覚えておく
			var selectedKey = ProjectSelectedItem?.Id;
			ProjectItemList = response.ProjectList;
			Project? reselectedItem = null;
			foreach (var item in ProjectItemList)
			{
				if (selectedKey != null && item.Id == selectedKey)
				{
					reselectedItem = item;
					break;
				}
			}

			// 選び直すとその行を要求に使うパーツが読み込まれるので、そちらの文面を消さないよう先に空にする
			ErrorMessage = string.Empty;
			ProjectSelectedItem = reselectedItem;
		}
		catch (AppException exception)
		{
			ProjectItemList = [];
			ErrorMessage = exception.Message;
		}
	}

	/// <summary>
	/// 月別バッジ件数表を読み込む
	/// </summary>
	private void LoadMonthlyBadgeCountTable()
	{
		if (ProjectSelectedItem == null)
		{
			MonthlyBadgeCountTableSource = CrossTable.Empty;
			return;
		}

		try
		{
			var request = new RequestMergeRequestGetMonthlyBadgeCountList();
			request.ProjectId = ProjectSelectedItem.ProjectId;
			var response = MergeRequestService.GetMonthlyBadgeCountList(request);
			var rowHeaderList = new List<string>();
			rowHeaderList.Add("年");
			rowHeaderList.Add("月");
			var cellList = new List<CrossTableCell>();
			foreach (var item in response.MonthlyBadgeCountList)
			{
				var cell = new CrossTableCell();
				cell.RowKeyList.Add(item.Year?.ToString() ?? string.Empty);
				cell.RowKeyList.Add(item.Month?.ToString() ?? string.Empty);
				cell.ColumnKey = item.BadgeName ?? string.Empty;
				cell.Value = item.Count ?? 0;
				cell.Ratio = item.Ratio;
				cellList.Add(cell);
			}

			MonthlyBadgeCountTableSource = CrossTable.Create(rowHeaderList, cellList);
			ErrorMessage = string.Empty;
		}
		catch (AppException exception)
		{
			MonthlyBadgeCountTableSource = CrossTable.Empty;
			ErrorMessage = exception.Message;
		}
	}

	/// <summary>
	/// 月別件数表を読み込む
	/// </summary>
	private void LoadMonthlyCountTable()
	{
		if (ProjectSelectedItem == null)
		{
			MonthlyCountTableSource = CrossTable.Empty;
			return;
		}

		try
		{
			var request = new RequestMergeRequestGetAuthorMonthlyCountList();
			request.ProjectId = ProjectSelectedItem.ProjectId;
			var response = MergeRequestService.GetAuthorMonthlyCountList(request);
			var rowHeaderList = new List<string>();
			rowHeaderList.Add("年");
			rowHeaderList.Add("月");
			var cellList = new List<CrossTableCell>();
			foreach (var item in response.MonthlyCountList)
			{
				var cell = new CrossTableCell();
				cell.RowKeyList.Add(item.Year?.ToString() ?? string.Empty);
				cell.RowKeyList.Add(item.Month?.ToString() ?? string.Empty);
				cell.ColumnKey = item.AuthorUsername ?? string.Empty;
				cell.Value = item.Count ?? 0;
				cellList.Add(cell);
			}

			MonthlyCountTableSource = CrossTable.Create(rowHeaderList, cellList);
			ErrorMessage = string.Empty;
		}
		catch (AppException exception)
		{
			MonthlyCountTableSource = CrossTable.Empty;
			ErrorMessage = exception.Message;
		}
	}
}
