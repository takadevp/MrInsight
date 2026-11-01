namespace MrInsight;

/// <summary>
/// 指定したプロジェクトの、すべてのマージリクエストのコメントを、マージリクエストの番号順・書かれた順に取得するレスポンス
/// </summary>
public class ResponseMergeRequestGetCommentDetailList
{
	/// <summary>
	/// コメントの明細一覧
	/// </summary>
	public List<MergeRequestCommentDetail> CommentDetailList { get; set; } = [];

}
