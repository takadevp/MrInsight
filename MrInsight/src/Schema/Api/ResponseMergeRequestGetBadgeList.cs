namespace MrInsight;

/// <summary>
/// 登録したバッジを、名前の順に取得するレスポンス
/// </summary>
public class ResponseMergeRequestGetBadgeList
{
	/// <summary>
	/// バッジ一覧
	/// </summary>
	public List<Badge> BadgeList { get; set; } = [];

}
