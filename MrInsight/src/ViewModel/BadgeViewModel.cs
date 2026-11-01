using App.Common;

namespace MrInsight;

/// <summary>
/// 集計に数えるバッジを登録し、消す。
/// コメントに書かれているが登録していないバッジも一覧にし、選んで登録できる
/// </summary>
public sealed class BadgeViewModel : ViewModelBase
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

	private string _addBadgeBadgeName = string.Empty;

	/// <summary>
	/// バッジの追加のバッジ名の入力欄
	/// </summary>
	public string AddBadgeBadgeName
	{
		get => _addBadgeBadgeName;
		set => SetProperty(ref _addBadgeBadgeName, value);
	}

	private RelayCommand? _addBadgeCommand;

	/// <summary>
	/// バッジの追加を実行するコマンド
	/// </summary>
	public RelayCommand AddBadgeCommand => _addBadgeCommand ??= new RelayCommand(ExecuteAddBadge, CanExecuteAddBadge);

	private List<Badge> _badgeTableItemList = [];

	private Badge? _badgeTableSelectedItem;

	/// <summary>
	/// バッジ表の行
	/// </summary>
	public List<Badge> BadgeTableItemList
	{
		get => _badgeTableItemList;
		private set => SetProperty(ref _badgeTableItemList, value);
	}

	/// <summary>
	/// バッジ表で選んだ行
	/// </summary>
	public Badge? BadgeTableSelectedItem
	{
		get => _badgeTableSelectedItem;
		set => SetProperty(ref _badgeTableSelectedItem, value);
	}

	private RelayCommand? _deleteBadgeCommand;

	/// <summary>
	/// バッジの削除を実行するコマンド
	/// </summary>
	public RelayCommand DeleteBadgeCommand => _deleteBadgeCommand ??= new RelayCommand(ExecuteDeleteBadge, CanExecuteDeleteBadge);

	private List<MergeRequestUnregisteredBadgeCount> _unregisteredBadgeTableItemList = [];

	private MergeRequestUnregisteredBadgeCount? _unregisteredBadgeTableSelectedItem;

	/// <summary>
	/// 未登録のバッジ表の行
	/// </summary>
	public List<MergeRequestUnregisteredBadgeCount> UnregisteredBadgeTableItemList
	{
		get => _unregisteredBadgeTableItemList;
		private set => SetProperty(ref _unregisteredBadgeTableItemList, value);
	}

	/// <summary>
	/// 未登録のバッジ表で選んだ行
	/// </summary>
	public MergeRequestUnregisteredBadgeCount? UnregisteredBadgeTableSelectedItem
	{
		get => _unregisteredBadgeTableSelectedItem;
		set => SetProperty(ref _unregisteredBadgeTableSelectedItem, value);
	}

	private RelayCommand? _registerBadgeCommand;

	/// <summary>
	/// 未登録のバッジの登録を実行するコマンド
	/// </summary>
	public RelayCommand RegisterBadgeCommand => _registerBadgeCommand ??= new RelayCommand(ExecuteRegisterBadge, CanExecuteRegisterBadge);

	/// <summary>
	/// 画面を開いたときに、要求の無いパーツを読み込む
	/// </summary>
	public BadgeViewModel()
	{
		LoadBadgeTable();
		LoadUnregisteredBadgeTable();
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
		LoadBadgeTable();
		LoadUnregisteredBadgeTable();
	}

	/// <summary>
	/// バッジの追加を押せるか
	/// </summary>
	private bool CanExecuteAddBadge()
	{
		if (IsRunning)
		{
			return false;
		}

		return true;
	}

	/// <summary>
	/// バッジの追加を実行する
	/// </summary>
	private void ExecuteAddBadge()
	{
		try
		{
			var request = new RequestMergeRequestAddBadge();
			request.BadgeName = AddBadgeBadgeName;
			MergeRequestService.AddBadge(request);
			ErrorMessage = string.Empty;
		}
		catch (AppException exception)
		{
			ErrorMessage = exception.Message;
			return;
		}

		AddBadgeBadgeName = string.Empty;

		LoadBadgeTable();

		LoadUnregisteredBadgeTable();
	}

	/// <summary>
	/// バッジ表を読み込む
	/// </summary>
	private void LoadBadgeTable()
	{
		try
		{
			var response = MergeRequestService.GetBadgeList();
			// 表は新しい一覧に置き換わると選択を外すので、置き換える前に選んでいた行の主キーを覚えておく
			var selectedKey = BadgeTableSelectedItem?.Id;
			BadgeTableItemList = response.BadgeList;
			Badge? reselectedItem = null;
			foreach (var item in BadgeTableItemList)
			{
				if (selectedKey != null && item.Id == selectedKey)
				{
					reselectedItem = item;
					break;
				}
			}

			// 選び直すとその行を要求に使うパーツが読み込まれるので、そちらの文面を消さないよう先に空にする
			ErrorMessage = string.Empty;
			BadgeTableSelectedItem = reselectedItem;
		}
		catch (AppException exception)
		{
			BadgeTableItemList = [];
			ErrorMessage = exception.Message;
		}
	}

	/// <summary>
	/// バッジの削除を押せるか
	/// </summary>
	private bool CanExecuteDeleteBadge()
	{
		if (IsRunning)
		{
			return false;
		}

		if (BadgeTableSelectedItem == null)
		{
			return false;
		}

		return true;
	}

	/// <summary>
	/// バッジの削除を実行する
	/// </summary>
	private void ExecuteDeleteBadge()
	{
		if (BadgeTableSelectedItem == null)
		{
			return;
		}

		try
		{
			var request = new RequestMergeRequestDeleteBadge();
			request.BadgeId = BadgeTableSelectedItem.Id;
			MergeRequestService.DeleteBadge(request);
			ErrorMessage = string.Empty;
		}
		catch (AppException exception)
		{
			ErrorMessage = exception.Message;
			return;
		}

		LoadBadgeTable();

		LoadUnregisteredBadgeTable();
	}

	/// <summary>
	/// 未登録のバッジ表を読み込む
	/// </summary>
	private void LoadUnregisteredBadgeTable()
	{
		try
		{
			var response = MergeRequestService.GetUnregisteredBadgeCountList();
			UnregisteredBadgeTableItemList = response.UnregisteredBadgeCountList;
			ErrorMessage = string.Empty;
		}
		catch (AppException exception)
		{
			UnregisteredBadgeTableItemList = [];
			ErrorMessage = exception.Message;
		}
	}

	/// <summary>
	/// 未登録のバッジの登録を押せるか
	/// </summary>
	private bool CanExecuteRegisterBadge()
	{
		if (IsRunning)
		{
			return false;
		}

		if (UnregisteredBadgeTableSelectedItem == null)
		{
			return false;
		}

		return true;
	}

	/// <summary>
	/// 未登録のバッジの登録を実行する
	/// </summary>
	private void ExecuteRegisterBadge()
	{
		if (UnregisteredBadgeTableSelectedItem == null)
		{
			return;
		}

		try
		{
			var request = new RequestMergeRequestAddBadge();
			request.BadgeName = UnregisteredBadgeTableSelectedItem.BadgeName;
			MergeRequestService.AddBadge(request);
			ErrorMessage = string.Empty;
		}
		catch (AppException exception)
		{
			ErrorMessage = exception.Message;
			return;
		}

		LoadBadgeTable();

		LoadUnregisteredBadgeTable();
	}
}
