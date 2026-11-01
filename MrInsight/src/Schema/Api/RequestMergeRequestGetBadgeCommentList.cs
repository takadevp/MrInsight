namespace MrInsight;

/// <summary>
/// 指定したプロジェクトの、指定した作成者のマージリクエストに、指定したバッジが付いたコメントを、新しい順に取得するリクエスト
/// </summary>
public class RequestMergeRequestGetBadgeCommentList
{
	/// <summary>
	/// プロジェクトID
	/// </summary>
	public int? ProjectId { get; set; }

	/// <summary>
	/// 作成者ユーザー名
	/// </summary>
	public string? AuthorUsername { get; set; }

	/// <summary>
	/// バッジ名
	/// </summary>
	public string? BadgeName { get; set; }

}
