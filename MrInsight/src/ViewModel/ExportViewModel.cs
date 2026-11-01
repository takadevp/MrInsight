using App.Common;

namespace MrInsight;

/// <summary>
/// プロジェクトを選んで、そのプロジェクトのコメントを CSV に書き出す。
/// 指摘を Excel で読み返したり、AI のスキルにする材料にしたりする
/// </summary>
public sealed class ExportViewModel : ViewModelBase
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
		set => SetProperty(ref _projectSelectedItem, value);
	}

	private RelayCommand? _exportCommentDetailCommand;

	/// <summary>
	/// コメントのCSV出力を実行するコマンド
	/// </summary>
	public RelayCommand ExportCommentDetailCommand => _exportCommentDetailCommand ??= new RelayCommand(ExecuteExportCommentDetail, CanExecuteExportCommentDetail);

	/// <summary>
	/// 画面を開いたときに、要求の無いパーツを読み込む
	/// </summary>
	public ExportViewModel()
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
	/// コメントのCSV出力を押せるか
	/// </summary>
	private bool CanExecuteExportCommentDetail()
	{
		if (IsRunning)
		{
			return false;
		}

		if (ProjectSelectedItem == null)
		{
			return false;
		}

		return true;
	}

	/// <summary>
	/// コメントのCSV出力を実行する
	/// </summary>
	private void ExecuteExportCommentDetail()
	{
		if (ProjectSelectedItem == null)
		{
			return;
		}

		try
		{
			var request = new RequestMergeRequestGetCommentDetailList();
			request.ProjectId = ProjectSelectedItem.ProjectId;
			var response = MergeRequestService.GetCommentDetailList(request);
			var headerList = new List<string>
			{
				"IID",
				"タイトル",
				"作成者ユーザー名",
				"ファイルパス",
				"行",
				"解決済み",
				"コメントした人のユーザー名",
				"作成日",
				"コメント本文",
				"バッジ名",
			};
			var rowList = new List<List<object?>>();
			foreach (var row in response.CommentDetailList)
			{
				rowList.Add(
				[
					row.Iid,
					row.Title,
					row.AuthorUsername,
					row.FilePath,
					row.LineNumber,
					row.Resolved,
					row.CommentAuthorUsername,
					row.CreatedAt,
					row.Body,
					row.BadgeName,
				]);
			}

			CsvFile.Save("コメントの明細", headerList, rowList);
			ErrorMessage = string.Empty;
		}
		catch (AppException exception)
		{
			ErrorMessage = exception.Message;
		}
	}
}
