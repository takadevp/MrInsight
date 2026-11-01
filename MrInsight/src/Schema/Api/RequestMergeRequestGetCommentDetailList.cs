namespace MrInsight;

/// <summary>
/// 指定したプロジェクトの、すべてのマージリクエストのコメントを、マージリクエストの番号順・書かれた順に取得するリクエスト
/// </summary>
public class RequestMergeRequestGetCommentDetailList
{
	/// <summary>
	/// プロジェクトID
	/// </summary>
	public int? ProjectId { get; set; }

}
