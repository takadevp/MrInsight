namespace MrInsight;

/// <summary>
/// 集計から除くタイトルの先頭を、文字の順に取得するレスポンス
/// </summary>
public class ResponseMergeRequestGetExcludedTitleList
{
	/// <summary>
	/// 除くタイトル一覧
	/// </summary>
	public List<ExcludedTitle> ExcludedTitleList { get; set; } = [];

}
