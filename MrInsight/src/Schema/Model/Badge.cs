namespace MrInsight;

/// <summary>
/// 集計に数えるバッジ1つ。
/// コメント本文に [design-badge] と書いたときの design の部分
/// </summary>
public class Badge
{
	/// <summary>
	/// ID
	/// </summary>
	public int? Id { get; set; }

	/// <summary>
	/// バッジ名
	/// </summary>
	public string? BadgeName { get; set; }

}
