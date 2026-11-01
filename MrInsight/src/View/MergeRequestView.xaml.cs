using System.Windows.Controls;

namespace MrInsight;

/// <summary>
/// プロジェクトを選んでマージリクエストを取り込み、そのプロジェクトのマージリクエストを一覧にする。
/// 選んだマージリクエストのスレッドと、選んだスレッドのコメントも一覧にする
/// </summary>
public partial class MergeRequestView : UserControl
{
	/// <summary>
	/// 画面を作り、ViewModel を結び付ける
	/// </summary>
	public MergeRequestView()
	{
		InitializeComponent();
		DataContext = new MergeRequestViewModel();
	}
}
