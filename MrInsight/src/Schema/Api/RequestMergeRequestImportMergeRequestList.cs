namespace MrInsight;

/// <summary>
/// 指定したプロジェクトの、前回から更新されたマージリクエストを GitLab から取り込むリクエスト
/// </summary>
public class RequestMergeRequestImportMergeRequestList
{
	/// <summary>
	/// プロジェクトID
	/// </summary>
	public int? ProjectId { get; set; }

}
