using System.Windows.Controls;

namespace MrInsight;

/// <summary>
/// 集計から除くマージリクエストを、タイトルの先頭の文字で登録し、消す。
/// 登録したものは、ほかの画面の集計と一覧から除かれる
/// </summary>
public partial class ExcludedTitleView : UserControl
{
	/// <summary>
	/// 画面を作り、ViewModel を結び付ける
	/// </summary>
	public ExcludedTitleView()
	{
		InitializeComponent();
		DataContext = new ExcludedTitleViewModel();
	}
}
