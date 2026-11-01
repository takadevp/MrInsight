namespace MrInsight;

/// <summary>
/// コメントに書かれているが登録していないバッジごとの件数。
/// 打ち間違いか、まだ登録していない観点
/// </summary>
public class MergeRequestUnregisteredBadgeCount
{
	/// <summary>
	/// バッジ名
	/// </summary>
	public string? BadgeName { get; set; }

	/// <summary>
	/// 件数
	/// </summary>
	public int? Count { get; set; }

}
