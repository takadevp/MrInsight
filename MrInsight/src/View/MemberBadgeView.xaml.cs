using System.Windows.Controls;

namespace MrInsight;

/// <summary>
/// プロジェクトと作成者を選んで、その作成者に多いバッジと、選んだバッジを受けた実際のコメントを出す。
/// 本人に伝える内容を決める
/// </summary>
public partial class MemberBadgeView : UserControl
{
	/// <summary>
	/// 画面を作り、ViewModel を結び付ける
	/// </summary>
	public MemberBadgeView()
	{
		InitializeComponent();
		DataContext = new MemberBadgeViewModel();
	}
}
