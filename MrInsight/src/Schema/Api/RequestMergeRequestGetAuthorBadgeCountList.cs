namespace MrInsight;

/// <summary>
/// 指定したプロジェクトの、指定した作成者のマージリクエストに付いたバッジごとの件数と、その作成者に付いたバッジ全体に対する割合を、多い順に取得するリクエスト
/// </summary>
public class RequestMergeRequestGetAuthorBadgeCountList
{
	/// <summary>
	/// プロジェクトID
	/// </summary>
	public int? ProjectId { get; set; }

	/// <summary>
	/// 作成者ユーザー名
	/// </summary>
	public string? AuthorUsername { get; set; }

}
