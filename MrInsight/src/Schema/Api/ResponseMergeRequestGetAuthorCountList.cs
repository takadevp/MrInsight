namespace MrInsight;

/// <summary>
/// 指定したプロジェクトの、作成者ごとのマージリクエストの件数を、多い順に取得するレスポンス
/// </summary>
public class ResponseMergeRequestGetAuthorCountList
{
	/// <summary>
	/// 件数一覧
	/// </summary>
	public List<MergeRequestAuthorCount> CountList { get; set; } = [];

}
