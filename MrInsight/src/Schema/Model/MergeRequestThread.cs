namespace MrInsight;

/// <summary>
/// マージリクエストのコメントのスレッド1つ。
/// コードへのコメントなら、ファイルと行を持つ
/// </summary>
public class MergeRequestThread
{
	/// <summary>
	/// ID
	/// </summary>
	public int? Id { get; set; }

	/// <summary>
	/// GitLab のディスカッションのID
	/// </summary>
	public string? GitlabThreadId { get; set; }

	/// <summary>
	/// 最初のコメントが書かれた日時
	/// </summary>
	public DateTime? CreatedAt { get; set; }

	/// <summary>
	/// 最初に書いた人のユーザー名
	/// </summary>
	public string? FirstAuthorUsername { get; set; }

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
	/// 解決済みにできるスレッドか
	/// </summary>
	public bool? Resolvable { get; set; }

	/// <summary>
	/// 解決済み
	/// </summary>
	public bool? Resolved { get; set; }

	/// <summary>
	/// スレッドの中の、人が書いたコメントの数
	/// </summary>
	public int? CommentCount { get; set; }

	/// <summary>
	/// コメント一覧
	/// </summary>
	public List<MergeRequestComment> CommentList { get; set; } = [];

}
