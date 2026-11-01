namespace MrInsight;

/// <summary>
/// 指定したプロジェクトの、作成者ごとのマージリクエストの件数を、多い順に取得するリクエスト
/// </summary>
public class RequestMergeRequestGetAuthorCountList
{
	/// <summary>
	/// プロジェクトID
	/// </summary>
	public int? ProjectId { get; set; }

}
