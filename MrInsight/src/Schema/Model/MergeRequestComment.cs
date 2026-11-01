namespace MrInsight;

/// <summary>
/// スレッドの中のコメント1件。
/// GitLab が自動で書く記録は含めない
/// </summary>
public class MergeRequestComment
{
	/// <summary>
	/// ID
	/// </summary>
	public int? Id { get; set; }

	/// <summary>
	/// GitLab のノートのID
	/// </summary>
	public int? GitlabCommentId { get; set; }

	/// <summary>
	/// 作成者ユーザー名
	/// </summary>
	public string? AuthorUsername { get; set; }

	/// <summary>
	/// 作成者名
	/// </summary>
	public string? AuthorName { get; set; }

	/// <summary>
	/// コメント本文
	/// </summary>
	public string? Body { get; set; }

	/// <summary>
	/// 作成日
	/// </summary>
	public DateTime? CreatedAt { get; set; }

	/// <summary>
	/// 更新日
	/// </summary>
	public DateTime? UpdatedAt { get; set; }

}
