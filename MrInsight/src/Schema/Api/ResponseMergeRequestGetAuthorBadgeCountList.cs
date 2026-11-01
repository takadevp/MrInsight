namespace MrInsight;

/// <summary>
/// 指定したプロジェクトの、指定した作成者のマージリクエストに付いたバッジごとの件数と、その作成者に付いたバッジ全体に対する割合を、多い順に取得するレスポンス
/// </summary>
public class ResponseMergeRequestGetAuthorBadgeCountList
{
	/// <summary>
	/// バッジ件数一覧
	/// </summary>
	public List<MergeRequestBadgeCount> BadgeCountList { get; set; } = [];

}
