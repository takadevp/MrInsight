using App.Common;

namespace MrInsight;

/// <summary>
/// プロジェクトを選んで、バッジごとの件数と割合と受けた人数を出す。
/// 多くの人が受けている指摘は、チームで仕組みにして防ぐ候補
/// </summary>
public sealed class TeamBadgeViewModel : ViewModelBase
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
				LoadTeamBadgeTable();
			}
		}
	}

	private List<MergeRequestTeamBadgeCount> _teamBadgeTableItemList = [];

	private MergeRequestTeamBadgeCount? _teamBadgeTableSelectedItem;

	/// <summary>
	/// 指摘表の行
	/// </summary>
	public List<MergeRequestTeamBadgeCount> TeamBadgeTableItemList
	{
		get => _teamBadgeTableItemList;
		private set => SetProperty(ref _teamBadgeTableItemList, value);
	}

	/// <summary>
	/// 指摘表で選んだ行
	/// </summary>
	public MergeRequestTeamBadgeCount? TeamBadgeTableSelectedItem
	{
		get => _teamBadgeTableSelectedItem;
		set => SetProperty(ref _teamBadgeTableSelectedItem, value);
	}

	/// <summary>
	/// 画面を開いたときに、要求の無いパーツを読み込む
	/// </summary>
	public TeamBadgeViewModel()
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
	/// 指摘表を読み込む
	/// </summary>
	private void LoadTeamBadgeTable()
	{
		if (ProjectSelectedItem == null)
		{
			TeamBadgeTableItemList = [];
			return;
		}

		try
		{
			var request = new RequestMergeRequestGetTeamBadgeCountList();
			request.ProjectId = ProjectSelectedItem.ProjectId;
			var response = MergeRequestService.GetTeamBadgeCountList(request);
			TeamBadgeTableItemList = response.TeamBadgeCountList;
			ErrorMessage = string.Empty;
		}
		catch (AppException exception)
		{
			TeamBadgeTableItemList = [];
			ErrorMessage = exception.Message;
		}
	}
}
