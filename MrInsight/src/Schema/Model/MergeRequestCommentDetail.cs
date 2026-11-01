namespace MrInsight;

/// <summary>
/// コメント1件ずつに、マージリクエストとスレッドの情報を添えたもの。
/// バッジの無いコメントも1行になる。
/// CSV に書き出して、指摘を読み返す材料にする
/// </summary>
public class MergeRequestCommentDetail
{
	/// <summary>
	/// IID
	/// </summary>
	public int? Iid { get; set; }

	/// <summary>
	/// タイトル
	/// </summary>
	public string? Title { get; set; }

	/// <summary>
	/// マージリクエストを作成した人のユーザー名
	/// </summary>
	public string? AuthorUsername { get; set; }

	/// <summary>
	/// コードへのコメントなら対象のファイル。
	/// 無ければ空
	/// </summary>
	public string? FilePath { get; set; }

	/// <summary>
	/// コードへのコメントなら対象の行。
	/// 無ければ空
	/// </summary>
	public int? LineNumber { get; set; }

	/// <summary>
	/// 解決済み
	/// </summary>
	public bool? Resolved { get; set; }

	/// <summary>
	/// コメントした人のユーザー名
	/// </summary>
	public string? CommentAuthorUsername { get; set; }

	/// <summary>
	/// コメントが書かれた日時
	/// </summary>
	public DateTime? CreatedAt { get; set; }

	/// <summary>
	/// コメント本文
	/// </summary>
	public string? Body { get; set; }

	/// <summary>
	/// コメント本文に書かれたバッジを空白で区切って並べたもの。
	/// 登録していないものも含める
	/// </summary>
	public string? BadgeName { get; set; }

}
