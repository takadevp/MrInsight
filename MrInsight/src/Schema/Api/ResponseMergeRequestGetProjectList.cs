namespace MrInsight;

/// <summary>
/// 取り込んだプロジェクトを取得するレスポンス
/// </summary>
public class ResponseMergeRequestGetProjectList
{
	/// <summary>
	/// プロジェクト一覧
	/// </summary>
	public List<Project> ProjectList { get; set; } = [];

}
