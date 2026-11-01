using App.Common;

namespace MrInsight;

/// <summary>
/// プロジェクトと作成者を選んで、その作成者に多いバッジと、選んだバッジを受けた実際のコメントを出す。
/// 本人に伝える内容を決める
/// </summary>
public sealed class MemberBadgeViewModel : ViewModelBase
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
				LoadCountTable();
				LoadAuthorBadgeTable();
				LoadBadgeCommentTable();
			}
		}
	}

	private List<MergeRequestAuthorCount> _countTableItemList = [];

	private MergeRequestAuthorCount? _countTableSelectedItem;

	/// <summary>
	/// 件数表の行
	/// </summary>
	public List<MergeRequestAuthorCount> CountTableItemList
	{
		get => _countTableItemList;
		private set => SetProperty(ref _countTableItemList, value);
	}

	/// <summary>
	/// 件数表で選んだ行
	/// </summary>
	public MergeRequestAuthorCount? CountTableSelectedItem
	{
		get => _countTableSelectedItem;
		set
		{
			if (SetProperty(ref _countTableSelectedItem, value))
			{
				LoadAuthorBadgeTable();
				LoadBadgeCommentTable();
			}
		}
	}

	private List<MergeRequestBadgeCount> _authorBadgeTableItemList = [];

	private MergeRequestBadgeCount? _authorBadgeTableSelectedItem;

	/// <summary>
	/// 作成者のバッジ件数表の行
	/// </summary>
	public List<MergeRequestBadgeCount> AuthorBadgeTableItemList
	{
		get => _authorBadgeTableItemList;
		private set => SetProperty(ref _authorBadgeTableItemList, value);
	}

	/// <summary>
	/// 作成者のバッジ件数表で選んだ行
	/// </summary>
	public MergeRequestBadgeCount? AuthorBadgeTableSelectedItem
	{
		get => _authorBadgeTableSelectedItem;
		set
		{
			if (SetProperty(ref _authorBadgeTableSelectedItem, value))
			{
				LoadBadgeCommentTable();
			}
		}
	}

	private List<MergeRequestBadgeComment> _badgeCommentTableItemList = [];

	private MergeRequestBadgeComment? _badgeCommentTableSelectedItem;

	/// <summary>
	/// 指摘のコメント表の行
	/// </summary>
	public List<MergeRequestBadgeComment> BadgeCommentTableItemList
	{
		get => _badgeCommentTableItemList;
		private set => SetProperty(ref _badgeCommentTableItemList, value);
	}

	/// <summary>
	/// 指摘のコメント表で選んだ行
	/// </summary>
	public MergeRequestBadgeComment? BadgeCommentTableSelectedItem
	{
		get => _badgeCommentTableSelectedItem;
		set => SetProperty(ref _badgeCommentTableSelectedItem, value);
	}

	/// <summary>
	/// 画面を開いたときに、要求の無いパーツを読み込む
	/// </summary>
	public MemberBadgeViewModel()
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
	/// 件数表を読み込む
	/// </summary>
	private void LoadCountTable()
	{
		if (ProjectSelectedItem == null)
		{
			CountTableItemList = [];
			return;
		}

		try
		{
			var request = new RequestMergeRequestGetAuthorCountList();
			request.ProjectId = ProjectSelectedItem.ProjectId;
			var response = MergeRequestService.GetAuthorCountList(request);
			CountTableItemList = response.CountList;
			ErrorMessage = string.Empty;
		}
		catch (AppException exception)
		{
			CountTableItemList = [];
			ErrorMessage = exception.Message;
		}
	}

	/// <summary>
	/// 作成者のバッジ件数表を読み込む
	/// </summary>
	private void LoadAuthorBadgeTable()
	{
		if (ProjectSelectedItem == null)
		{
			AuthorBadgeTableItemList = [];
			return;
		}

		if (CountTableSelectedItem == null)
		{
			AuthorBadgeTableItemList = [];
			return;
		}

		try
		{
			var request = new RequestMergeRequestGetAuthorBadgeCountList();
			request.ProjectId = ProjectSelectedItem.ProjectId;
			request.AuthorUsername = CountTableSelectedItem.AuthorUsername;
			var response = MergeRequestService.GetAuthorBadgeCountList(request);
			AuthorBadgeTableItemList = response.BadgeCountList;
			ErrorMessage = string.Empty;
		}
		catch (AppException exception)
		{
			AuthorBadgeTableItemList = [];
			ErrorMessage = exception.Message;
		}
	}

	/// <summary>
	/// 指摘のコメント表を読み込む
	/// </summary>
	private void LoadBadgeCommentTable()
	{
		if (ProjectSelectedItem == null)
		{
			BadgeCommentTableItemList = [];
			return;
		}

		if (CountTableSelectedItem == null)
		{
			BadgeCommentTableItemList = [];
			return;
		}

		if (AuthorBadgeTableSelectedItem == null)
		{
			BadgeCommentTableItemList = [];
			return;
		}

		try
		{
			var request = new RequestMergeRequestGetBadgeCommentList();
			request.ProjectId = ProjectSelectedItem.ProjectId;
			request.AuthorUsername = CountTableSelectedItem.AuthorUsername;
			request.BadgeName = AuthorBadgeTableSelectedItem.BadgeName;
			var response = MergeRequestService.GetBadgeCommentList(request);
			BadgeCommentTableItemList = response.BadgeCommentList;
			ErrorMessage = string.Empty;
		}
		catch (AppException exception)
		{
			BadgeCommentTableItemList = [];
			ErrorMessage = exception.Message;
		}
	}
}
