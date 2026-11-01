namespace MrInsight;

/// <summary>
/// 指定したプロジェクトの、作成月と作成者ごとのマージリクエストの件数を、新しい月から取得するレスポンス
/// </summary>
public class ResponseMergeRequestGetAuthorMonthlyCountList
{
	/// <summary>
	/// 月別件数一覧
	/// </summary>
	public List<MergeRequestAuthorMonthlyCount> MonthlyCountList { get; set; } = [];

}
