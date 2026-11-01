namespace MrInsight;

/// <summary>
/// GitLab から取り込んだプロジェクト1つ
/// </summary>
public class Project
{
	/// <summary>
	/// ID
	/// </summary>
	public int? Id { get; set; }

	/// <summary>
	/// GitLab のプロジェクトのID
	/// </summary>
	public int? ProjectId { get; set; }

	/// <summary>
	/// プロジェクト名
	/// </summary>
	public string? ProjectName { get; set; }

	/// <summary>
	/// グループを含めたパス
	/// </summary>
	public string? ProjectPath { get; set; }

	/// <summary>
	/// GitLab の画面のURL
	/// </summary>
	public string? WebUrl { get; set; }

}
