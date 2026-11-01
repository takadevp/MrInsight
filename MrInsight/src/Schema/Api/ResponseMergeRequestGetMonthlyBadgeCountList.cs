namespace MrInsight;

/// <summary>
/// 指定したプロジェクトの、コメントが書かれた月とバッジごとの件数と割合を、新しい月から取得するレスポンス
/// </summary>
public class ResponseMergeRequestGetMonthlyBadgeCountList
{
	/// <summary>
	/// 月別バッジ件数一覧
	/// </summary>
	public List<MergeRequestMonthlyBadgeCount> MonthlyBadgeCountList { get; set; } = [];

}
