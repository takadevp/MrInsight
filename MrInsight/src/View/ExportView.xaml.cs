using System.Windows.Controls;

namespace MrInsight;

/// <summary>
/// プロジェクトを選んで、そのプロジェクトのコメントを CSV に書き出す。
/// 指摘を Excel で読み返したり、AI のスキルにする材料にしたりする
/// </summary>
public partial class ExportView : UserControl
{
	/// <summary>
	/// 画面を作り、ViewModel を結び付ける
	/// </summary>
	public ExportView()
	{
		InitializeComponent();
		DataContext = new ExportViewModel();
	}
}
