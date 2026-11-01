namespace MrInsight;

/// <summary>
/// 指定したプロジェクトのマージリクエストを、新しい順に取得するレスポンス
/// </summary>
public class ResponseMergeRequestGetProjectMergeRequestList
{
	/// <summary>
	/// マージリクエスト一覧
	/// </summary>
	public List<MergeRequest> MergeRequestList { get; set; } = [];

}
