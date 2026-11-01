namespace MrInsight;

/// <summary>
/// 指定したプロジェクトのマージリクエストを、新しい順に取得するリクエスト
/// </summary>
public class RequestMergeRequestGetProjectMergeRequestList
{
	/// <summary>
	/// プロジェクトID
	/// </summary>
	public int? ProjectId { get; set; }

}
