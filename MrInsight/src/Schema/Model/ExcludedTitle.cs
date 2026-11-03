namespace MrInsight;

/// <summary>
/// 集計から除くマージリクエストの、タイトルの先頭の文字1つ
/// </summary>
public class ExcludedTitle
{
	/// <summary>
	/// ID
	/// </summary>
	public int? Id { get; set; }

	/// <summary>
	/// タイトルがこの文字で始まるマージリクエストを除く
	/// </summary>
	public string? TitlePrefix { get; set; }

}
