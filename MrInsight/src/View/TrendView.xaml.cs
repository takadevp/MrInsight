using System.Windows.Controls;

namespace MrInsight;

/// <summary>
/// プロジェクトを選んで、月ごとに付いたバッジの件数と割合と、月ごとの作成者のマージリクエストの件数を出す。
/// 指摘が減ってきたかを見る
/// </summary>
public partial class TrendView : UserControl
{
	/// <summary>
	/// 画面を作り、ViewModel を結び付ける
	/// </summary>
	public TrendView()
	{
		InitializeComponent();
		DataContext = new TrendViewModel();
	}
}
