namespace MrInsight;

/// <summary>
/// マージリクエストのレビューを頼まれた人1人
/// </summary>
public class MergeRequestReviewer
{
	/// <summary>
	/// ID
	/// </summary>
	public int? Id { get; set; }

	/// <summary>
	/// GitLab のユーザーID
	/// </summary>
	public int? ReviewerUserId { get; set; }

	/// <summary>
	/// レビュアーユーザー名
	/// </summary>
	public string? ReviewerUsername { get; set; }

}
