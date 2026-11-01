namespace MrInsight;

/// <summary>
/// 登録したバッジが付いたコメント1件ずつに、マージリクエストの情報を添えたもの。
/// 1件のコメントに2つのバッジがあれば2行になる
/// </summary>
public class MergeRequestBadgeComment
{
	/// <summary>
	/// コメントが書かれた日時
	/// </summary>
	public DateTime? CreatedAt { get; set; }

	/// <summary>
	/// IID
	/// </summary>
	public int? Iid { get; set; }

	/// <summary>
	/// タイトル
	/// </summary>
	public string? Title { get; set; }

	/// <summary>
	/// コメント本文
	/// </summary>
	public string? Body { get; set; }

	/// <summary>
	/// バッジ名
	/// </summary>
	public string? BadgeName { get; set; }

}
