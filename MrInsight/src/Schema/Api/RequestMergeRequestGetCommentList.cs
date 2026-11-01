namespace MrInsight;

/// <summary>
/// 指定したスレッドのコメントを、古い順に取得するリクエスト
/// </summary>
public class RequestMergeRequestGetCommentList
{
	/// <summary>
	/// スレッドID
	/// </summary>
	public int? ThreadId { get; set; }

}
