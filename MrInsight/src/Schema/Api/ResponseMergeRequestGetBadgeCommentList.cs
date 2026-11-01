namespace MrInsight;

/// <summary>
/// 指定したプロジェクトの、指定した作成者のマージリクエストに、指定したバッジが付いたコメントを、新しい順に取得するレスポンス
/// </summary>
public class ResponseMergeRequestGetBadgeCommentList
{
	/// <summary>
	/// バッジの付いたコメント一覧
	/// </summary>
	public List<MergeRequestBadgeComment> BadgeCommentList { get; set; } = [];

}
