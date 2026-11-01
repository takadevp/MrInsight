namespace MrInsight;

/// <summary>
/// 指定したスレッドのコメントを、古い順に取得するレスポンス
/// </summary>
public class ResponseMergeRequestGetCommentList
{
	/// <summary>
	/// コメント一覧
	/// </summary>
	public List<MergeRequestComment> CommentList { get; set; } = [];

}
