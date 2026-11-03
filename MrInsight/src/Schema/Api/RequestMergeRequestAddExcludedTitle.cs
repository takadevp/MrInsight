namespace MrInsight;

/// <summary>
/// 集計から除くタイトルの先頭を1つ登録するリクエスト
/// </summary>
public class RequestMergeRequestAddExcludedTitle
{
	/// <summary>
	/// タイトルの先頭
	/// </summary>
	public string? TitlePrefix { get; set; }

}
