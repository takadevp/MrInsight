namespace MrInsight;

/// <summary>
/// 指定したプロジェクトの、バッジごとの件数と割合と受けた人数を、受けた人数の多い順に取得するリクエスト
/// </summary>
public class RequestMergeRequestGetTeamBadgeCountList
{
	/// <summary>
	/// プロジェクトID
	/// </summary>
	public int? ProjectId { get; set; }

}
