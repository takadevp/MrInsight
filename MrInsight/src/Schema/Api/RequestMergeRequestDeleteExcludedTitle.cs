namespace MrInsight;

/// <summary>
/// 登録した除くタイトルの先頭を1つ消すリクエスト
/// </summary>
public class RequestMergeRequestDeleteExcludedTitle
{
	/// <summary>
	/// 除くタイトルID
	/// </summary>
	public int? ExcludedTitleId { get; set; }

}
