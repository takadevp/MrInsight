namespace MrInsight;

/// <summary>
/// 指定したプロジェクトのマージリクエストのうち、タイトル・本文・作成者ユーザー名に指定した文字を含み、作成日が指定した期間に入るものを、新しい順に取得するリクエスト
/// </summary>
public class RequestMergeRequestGetProjectMergeRequestList
{
	/// <summary>
	/// プロジェクトID
	/// </summary>
	public int? ProjectId { get; set; }

	/// <summary>
	/// タイトルに含む文字
	/// </summary>
	public string? Title { get; set; }

	/// <summary>
	/// 本文に含む文字
	/// </summary>
	public string? Description { get; set; }

	/// <summary>
	/// 作成者ユーザー名に含む文字
	/// </summary>
	public string? AuthorUsername { get; set; }

	/// <summary>
	/// この日以降に作成されたもの
	/// </summary>
	public DateTime? StartDate { get; set; }

	/// <summary>
	/// この日までに作成されたもの
	/// </summary>
	public DateTime? EndDate { get; set; }

}
