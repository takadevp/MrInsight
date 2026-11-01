namespace MrInsight;

/// <summary>
/// 作成月と作成者ごとのマージリクエストの件数
/// </summary>
public class MergeRequestAuthorMonthlyCount
{
	/// <summary>
	/// 年
	/// </summary>
	public int? Year { get; set; }

	/// <summary>
	/// 月
	/// </summary>
	public int? Month { get; set; }

	/// <summary>
	/// 作成者ユーザー名
	/// </summary>
	public string? AuthorUsername { get; set; }

	/// <summary>
	/// 件数
	/// </summary>
	public int? Count { get; set; }

}
