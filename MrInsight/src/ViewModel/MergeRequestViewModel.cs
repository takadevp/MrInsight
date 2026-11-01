using App.Common;

namespace MrInsight;

/// <summary>
/// プロジェクトを選んでマージリクエストを取り込み、そのプロジェクトのマージリクエストを一覧にする。
/// 選んだマージリクエストのスレッドと、選んだスレッドのコメントも一覧にする
/// </summary>
public sealed class MergeRequestViewModel : ViewModelBase
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

	private RelayCommand? _importProjectCommand;

	/// <summary>
	/// プロジェクトの取り込みを実行するコマンド
	/// </summary>
	public RelayCommand ImportProjectCommand => _importProjectCommand ??= new RelayCommand(ExecuteImportProject, CanExecuteImportProject);

	private CancellationTokenSource? _importProjectCancellation;

	private RelayCommand? _importProjectCancelCommand;

	/// <summary>
	/// プロジェクトの取り込みをキャンセルするコマンド
	/// </summary>
	public RelayCommand ImportProjectCancelCommand => _importProjectCancelCommand ??= new RelayCommand(ExecuteImportProjectCancel, CanExecuteImportProjectCancel);

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
				LoadMergeRequestTable();
			}
		}
	}

	private RelayCommand? _importMergeRequestCommand;

	/// <summary>
	/// マージリクエストの取り込みを実行するコマンド
	/// </summary>
	public RelayCommand ImportMergeRequestCommand => _importMergeRequestCommand ??= new RelayCommand(ExecuteImportMergeRequest, CanExecuteImportMergeRequest);

	private CancellationTokenSource? _importMergeRequestCancellation;

	private RelayCommand? _importMergeRequestCancelCommand;

	/// <summary>
	/// マージリクエストの取り込みをキャンセルするコマンド
	/// </summary>
	public RelayCommand ImportMergeRequestCancelCommand => _importMergeRequestCancelCommand ??= new RelayCommand(ExecuteImportMergeRequestCancel, CanExecuteImportMergeRequestCancel);

	private List<MergeRequest> _mergeRequestTableItemList = [];

	private MergeRequest? _mergeRequestTableSelectedItem;

	/// <summary>
	/// マージリクエスト表の行
	/// </summary>
	public List<MergeRequest> MergeRequestTableItemList
	{
		get => _mergeRequestTableItemList;
		private set => SetProperty(ref _mergeRequestTableItemList, value);
	}

	/// <summary>
	/// マージリクエスト表で選んだ行
	/// </summary>
	public MergeRequest? MergeRequestTableSelectedItem
	{
		get => _mergeRequestTableSelectedItem;
		set
		{
			if (SetProperty(ref _mergeRequestTableSelectedItem, value))
			{
				LoadThreadTable();
			}
		}
	}

	private List<MergeRequestThread> _threadTableItemList = [];

	private MergeRequestThread? _threadTableSelectedItem;

	/// <summary>
	/// スレッド表の行
	/// </summary>
	public List<MergeRequestThread> ThreadTableItemList
	{
		get => _threadTableItemList;
		private set => SetProperty(ref _threadTableItemList, value);
	}

	/// <summary>
	/// スレッド表で選んだ行
	/// </summary>
	public MergeRequestThread? ThreadTableSelectedItem
	{
		get => _threadTableSelectedItem;
		set
		{
			if (SetProperty(ref _threadTableSelectedItem, value))
			{
				LoadCommentTable();
			}
		}
	}

	private List<MergeRequestComment> _commentTableItemList = [];

	private MergeRequestComment? _commentTableSelectedItem;

	/// <summary>
	/// コメント表の行
	/// </summary>
	public List<MergeRequestComment> CommentTableItemList
	{
		get => _commentTableItemList;
		private set => SetProperty(ref _commentTableItemList, value);
	}

	/// <summary>
	/// コメント表で選んだ行
	/// </summary>
	public MergeRequestComment? CommentTableSelectedItem
	{
		get => _commentTableSelectedItem;
		set => SetProperty(ref _commentTableSelectedItem, value);
	}

	/// <summary>
	/// 画面を開いたときに、要求の無いパーツを読み込む
	/// </summary>
	public MergeRequestViewModel()
	{
		LoadProject();
	}

	/// <summary>
	/// プロジェクトの取り込みを押せるか
	/// </summary>
	private bool CanExecuteImportProject()
	{
		if (IsRunning)
		{
			return false;
		}

		return true;
	}

	/// <summary>
	/// プロジェクトの取り込みを実行する
	/// </summary>
	private async void ExecuteImportProject()
	{
		IsRunning = true;
		var cancellation = new CancellationTokenSource();
		_importProjectCancellation = cancellation;
		ImportProjectCommand.RaiseCanExecuteChanged();
		try
		{
			await MergeRequestService.ImportProjectList(cancellation.Token);
			ErrorMessage = string.Empty;
		}
		catch (AppException exception)
		{
			ErrorMessage = exception.Message;
			return;
		}
		catch (OperationCanceledException)
		{
			ErrorMessage = "プロジェクトの取り込みをキャンセルしました。";
			return;
		}
		finally
		{
			IsRunning = false;
			_importProjectCancellation = null;
			cancellation.Dispose();
			ImportProjectCommand.RaiseCanExecuteChanged();
		}

		LoadProject();
	}

	/// <summary>
	/// プロジェクトの取り込みをキャンセルできるか
	/// </summary>
	private bool CanExecuteImportProjectCancel()
	{
		if (_importProjectCancellation == null)
		{
			return false;
		}

		return true;
	}

	/// <summary>
	/// プロジェクトの取り込みをキャンセルする
	/// </summary>
	private void ExecuteImportProjectCancel()
	{
		_importProjectCancellation?.Cancel();
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
	/// マージリクエストの取り込みを押せるか
	/// </summary>
	private bool CanExecuteImportMergeRequest()
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
	/// マージリクエストの取り込みを実行する
	/// </summary>
	private async void ExecuteImportMergeRequest()
	{
		if (ProjectSelectedItem == null)
		{
			return;
		}

		IsRunning = true;
		var cancellation = new CancellationTokenSource();
		_importMergeRequestCancellation = cancellation;
		ImportMergeRequestCommand.RaiseCanExecuteChanged();
		try
		{
			var request = new RequestMergeRequestImportMergeRequestList();
			request.ProjectId = ProjectSelectedItem.ProjectId;
			await MergeRequestService.ImportMergeRequestList(request, cancellation.Token);
			ErrorMessage = string.Empty;
		}
		catch (AppException exception)
		{
			ErrorMessage = exception.Message;
			return;
		}
		catch (OperationCanceledException)
		{
			ErrorMessage = "マージリクエストの取り込みをキャンセルしました。";
			return;
		}
		finally
		{
			IsRunning = false;
			_importMergeRequestCancellation = null;
			cancellation.Dispose();
			ImportMergeRequestCommand.RaiseCanExecuteChanged();
		}

		LoadMergeRequestTable();
	}

	/// <summary>
	/// マージリクエストの取り込みをキャンセルできるか
	/// </summary>
	private bool CanExecuteImportMergeRequestCancel()
	{
		if (_importMergeRequestCancellation == null)
		{
			return false;
		}

		return true;
	}

	/// <summary>
	/// マージリクエストの取り込みをキャンセルする
	/// </summary>
	private void ExecuteImportMergeRequestCancel()
	{
		_importMergeRequestCancellation?.Cancel();
	}

	/// <summary>
	/// マージリクエスト表を読み込む
	/// </summary>
	private void LoadMergeRequestTable()
	{
		if (ProjectSelectedItem == null)
		{
			MergeRequestTableItemList = [];
			return;
		}

		try
		{
			var request = new RequestMergeRequestGetProjectMergeRequestList();
			request.ProjectId = ProjectSelectedItem.ProjectId;
			var response = MergeRequestService.GetProjectMergeRequestList(request);
			// 表は新しい一覧に置き換わると選択を外すので、置き換える前に選んでいた行の主キーを覚えておく
			var selectedKey = MergeRequestTableSelectedItem?.Id;
			MergeRequestTableItemList = response.MergeRequestList;
			MergeRequest? reselectedItem = null;
			foreach (var item in MergeRequestTableItemList)
			{
				if (selectedKey != null && item.Id == selectedKey)
				{
					reselectedItem = item;
					break;
				}
			}

			// 選び直すとその行を要求に使うパーツが読み込まれるので、そちらの文面を消さないよう先に空にする
			ErrorMessage = string.Empty;
			MergeRequestTableSelectedItem = reselectedItem;
		}
		catch (AppException exception)
		{
			MergeRequestTableItemList = [];
			ErrorMessage = exception.Message;
		}
	}

	/// <summary>
	/// スレッド表を読み込む
	/// </summary>
	private void LoadThreadTable()
	{
		if (MergeRequestTableSelectedItem == null)
		{
			ThreadTableItemList = [];
			return;
		}

		try
		{
			var request = new RequestMergeRequestGetThreadList();
			request.MergeRequestId = MergeRequestTableSelectedItem.Id;
			var response = MergeRequestService.GetThreadList(request);
			// 表は新しい一覧に置き換わると選択を外すので、置き換える前に選んでいた行の主キーを覚えておく
			var selectedKey = ThreadTableSelectedItem?.Id;
			ThreadTableItemList = response.ThreadList;
			MergeRequestThread? reselectedItem = null;
			foreach (var item in ThreadTableItemList)
			{
				if (selectedKey != null && item.Id == selectedKey)
				{
					reselectedItem = item;
					break;
				}
			}

			// 選び直すとその行を要求に使うパーツが読み込まれるので、そちらの文面を消さないよう先に空にする
			ErrorMessage = string.Empty;
			ThreadTableSelectedItem = reselectedItem;
		}
		catch (AppException exception)
		{
			ThreadTableItemList = [];
			ErrorMessage = exception.Message;
		}
	}

	/// <summary>
	/// コメント表を読み込む
	/// </summary>
	private void LoadCommentTable()
	{
		if (ThreadTableSelectedItem == null)
		{
			CommentTableItemList = [];
			return;
		}

		try
		{
			var request = new RequestMergeRequestGetCommentList();
			request.ThreadId = ThreadTableSelectedItem.Id;
			var response = MergeRequestService.GetCommentList(request);
			// 表は新しい一覧に置き換わると選択を外すので、置き換える前に選んでいた行の主キーを覚えておく
			var selectedKey = CommentTableSelectedItem?.Id;
			CommentTableItemList = response.CommentList;
			MergeRequestComment? reselectedItem = null;
			foreach (var item in CommentTableItemList)
			{
				if (selectedKey != null && item.Id == selectedKey)
				{
					reselectedItem = item;
					break;
				}
			}

			// 選び直すとその行を要求に使うパーツが読み込まれるので、そちらの文面を消さないよう先に空にする
			ErrorMessage = string.Empty;
			CommentTableSelectedItem = reselectedItem;
		}
		catch (AppException exception)
		{
			CommentTableItemList = [];
			ErrorMessage = exception.Message;
		}
	}
}
