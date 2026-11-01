namespace MrInsight;

/// <summary>
/// 作成者ごとのマージリクエストの件数
/// </summary>
public class MergeRequestAuthorCount
{
	/// <summary>
	/// 作成者ユーザー名
	/// </summary>
	public string? AuthorUsername { get; set; }

	/// <summary>
	/// 件数
	/// </summary>
	public int? Count { get; set; }

}
