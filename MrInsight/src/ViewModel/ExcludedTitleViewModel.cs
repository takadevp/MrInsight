using App.Common;

namespace MrInsight;

/// <summary>
/// 集計から除くマージリクエストを、タイトルの先頭の文字で登録し、消す。
/// 登録したものは、ほかの画面の集計と一覧から除かれる
/// </summary>
public sealed class ExcludedTitleViewModel : ViewModelBase
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

	private string _addExcludedTitleTitlePrefix = string.Empty;

	/// <summary>
	/// 除くタイトルの追加のタイトルの先頭の入力欄
	/// </summary>
	public string AddExcludedTitleTitlePrefix
	{
		get => _addExcludedTitleTitlePrefix;
		set => SetProperty(ref _addExcludedTitleTitlePrefix, value);
	}

	private RelayCommand? _addExcludedTitleCommand;

	/// <summary>
	/// 除くタイトルの追加を実行するコマンド
	/// </summary>
	public RelayCommand AddExcludedTitleCommand => _addExcludedTitleCommand ??= new RelayCommand(ExecuteAddExcludedTitle, CanExecuteAddExcludedTitle);

	private List<ExcludedTitle> _excludedTitleTableItemList = [];

	private ExcludedTitle? _excludedTitleTableSelectedItem;

	/// <summary>
	/// 除くタイトル表の行
	/// </summary>
	public List<ExcludedTitle> ExcludedTitleTableItemList
	{
		get => _excludedTitleTableItemList;
		private set => SetProperty(ref _excludedTitleTableItemList, value);
	}

	/// <summary>
	/// 除くタイトル表で選んだ行
	/// </summary>
	public ExcludedTitle? ExcludedTitleTableSelectedItem
	{
		get => _excludedTitleTableSelectedItem;
		set => SetProperty(ref _excludedTitleTableSelectedItem, value);
	}

	private RelayCommand? _deleteExcludedTitleCommand;

	/// <summary>
	/// 除くタイトルの削除を実行するコマンド
	/// </summary>
	public RelayCommand DeleteExcludedTitleCommand => _deleteExcludedTitleCommand ??= new RelayCommand(ExecuteDeleteExcludedTitle, CanExecuteDeleteExcludedTitle);

	/// <summary>
	/// 画面を開いたときに、要求の無いパーツを読み込む
	/// </summary>
	public ExcludedTitleViewModel()
	{
		LoadExcludedTitleTable();
	}

	/// <summary>
	/// 除くタイトルの追加を押せるか
	/// </summary>
	private bool CanExecuteAddExcludedTitle()
	{
		if (IsRunning)
		{
			return false;
		}

		return true;
	}

	/// <summary>
	/// 除くタイトルの追加を実行する
	/// </summary>
	private void ExecuteAddExcludedTitle()
	{
		try
		{
			var request = new RequestMergeRequestAddExcludedTitle();
			request.TitlePrefix = AddExcludedTitleTitlePrefix;
			MergeRequestService.AddExcludedTitle(request);
			ErrorMessage = string.Empty;
		}
		catch (AppException exception)
		{
			ErrorMessage = exception.Message;
			return;
		}

		AddExcludedTitleTitlePrefix = string.Empty;

		LoadExcludedTitleTable();
	}

	/// <summary>
	/// 除くタイトル表を読み込む
	/// </summary>
	private void LoadExcludedTitleTable()
	{
		try
		{
			var response = MergeRequestService.GetExcludedTitleList();
			// 表は新しい一覧に置き換わると選択を外すので、置き換える前に選んでいた行の主キーを覚えておく
			var selectedKey = ExcludedTitleTableSelectedItem?.Id;
			ExcludedTitleTableItemList = response.ExcludedTitleList;
			ExcludedTitle? reselectedItem = null;
			foreach (var item in ExcludedTitleTableItemList)
			{
				if (selectedKey != null && item.Id == selectedKey)
				{
					reselectedItem = item;
					break;
				}
			}

			// 選び直すとその行を要求に使うパーツが読み込まれるので、そちらの文面を消さないよう先に空にする
			ErrorMessage = string.Empty;
			ExcludedTitleTableSelectedItem = reselectedItem;
		}
		catch (AppException exception)
		{
			ExcludedTitleTableItemList = [];
			ErrorMessage = exception.Message;
		}
	}

	/// <summary>
	/// 除くタイトルの削除を押せるか
	/// </summary>
	private bool CanExecuteDeleteExcludedTitle()
	{
		if (IsRunning)
		{
			return false;
		}

		if (ExcludedTitleTableSelectedItem == null)
		{
			return false;
		}

		return true;
	}

	/// <summary>
	/// 除くタイトルの削除を実行する
	/// </summary>
	private void ExecuteDeleteExcludedTitle()
	{
		if (ExcludedTitleTableSelectedItem == null)
		{
			return;
		}

		try
		{
			var request = new RequestMergeRequestDeleteExcludedTitle();
			request.ExcludedTitleId = ExcludedTitleTableSelectedItem.Id;
			MergeRequestService.DeleteExcludedTitle(request);
			ErrorMessage = string.Empty;
		}
		catch (AppException exception)
		{
			ErrorMessage = exception.Message;
			return;
		}

		LoadExcludedTitleTable();
	}
}
