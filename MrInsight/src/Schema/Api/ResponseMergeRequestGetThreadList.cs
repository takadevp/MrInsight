namespace MrInsight;

/// <summary>
/// 指定したマージリクエストのスレッドを、古い順に取得するレスポンス
/// </summary>
public class ResponseMergeRequestGetThreadList
{
	/// <summary>
	/// スレッド一覧
	/// </summary>
	public List<MergeRequestThread> ThreadList { get; set; } = [];

}
