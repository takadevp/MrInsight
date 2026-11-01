using System.Windows.Controls;

namespace MrInsight;

/// <summary>
/// 集計に数えるバッジを登録し、消す。
/// コメントに書かれているが登録していないバッジも一覧にし、選んで登録できる
/// </summary>
public partial class BadgeView : UserControl
{
	/// <summary>
	/// 画面を作り、ViewModel を結び付ける
	/// </summary>
	public BadgeView()
	{
		InitializeComponent();
		DataContext = new BadgeViewModel();
	}
}
