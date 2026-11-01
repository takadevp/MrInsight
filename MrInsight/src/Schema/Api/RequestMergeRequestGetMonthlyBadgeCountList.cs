namespace MrInsight;

/// <summary>
/// 指定したプロジェクトの、コメントが書かれた月とバッジごとの件数と割合を、新しい月から取得するリクエスト
/// </summary>
public class RequestMergeRequestGetMonthlyBadgeCountList
{
	/// <summary>
	/// プロジェクトID
	/// </summary>
	public int? ProjectId { get; set; }

}
