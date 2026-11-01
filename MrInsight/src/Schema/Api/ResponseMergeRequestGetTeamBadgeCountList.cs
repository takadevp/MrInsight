namespace MrInsight;

/// <summary>
/// 指定したプロジェクトの、バッジごとの件数と割合と受けた人数を、受けた人数の多い順に取得するレスポンス
/// </summary>
public class ResponseMergeRequestGetTeamBadgeCountList
{
	/// <summary>
	/// チームのバッジ件数一覧
	/// </summary>
	public List<MergeRequestTeamBadgeCount> TeamBadgeCountList { get; set; } = [];

}
