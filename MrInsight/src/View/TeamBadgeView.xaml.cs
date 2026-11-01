using System.Windows.Controls;

namespace MrInsight;

/// <summary>
/// プロジェクトを選んで、バッジごとの件数と割合と受けた人数を出す。
/// 多くの人が受けている指摘は、チームで仕組みにして防ぐ候補
/// </summary>
public partial class TeamBadgeView : UserControl
{
	/// <summary>
	/// 画面を作り、ViewModel を結び付ける
	/// </summary>
	public TeamBadgeView()
	{
		InitializeComponent();
		DataContext = new TeamBadgeViewModel();
	}
}
