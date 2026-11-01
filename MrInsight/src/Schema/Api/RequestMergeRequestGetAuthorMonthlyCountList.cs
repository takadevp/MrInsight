namespace MrInsight;

/// <summary>
/// 指定したプロジェクトの、作成月と作成者ごとのマージリクエストの件数を、新しい月から取得するリクエスト
/// </summary>
public class RequestMergeRequestGetAuthorMonthlyCountList
{
	/// <summary>
	/// プロジェクトID
	/// </summary>
	public int? ProjectId { get; set; }

}
