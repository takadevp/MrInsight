namespace MrInsight;

/// <summary>
/// 指定したマージリクエストのスレッドを、古い順に取得するリクエスト
/// </summary>
public class RequestMergeRequestGetThreadList
{
	/// <summary>
	/// マージリクエストID
	/// </summary>
	public int? MergeRequestId { get; set; }

}
