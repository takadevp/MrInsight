namespace MrInsight;

/// <summary>
/// すべてのプロジェクトのコメントから、登録していないバッジごとの件数を、多い順に取得するレスポンス
/// </summary>
public class ResponseMergeRequestGetUnregisteredBadgeCountList
{
	/// <summary>
	/// 未登録のバッジ件数一覧
	/// </summary>
	public List<MergeRequestUnregisteredBadgeCount> UnregisteredBadgeCountList { get; set; } = [];

}
