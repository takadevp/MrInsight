namespace MrInsight;

/// <summary>
/// マージリクエストに割り当てられた人1人
/// </summary>
public class MergeRequestAssignee
{
	/// <summary>
	/// ID
	/// </summary>
	public int? Id { get; set; }

	/// <summary>
	/// GitLab のユーザーID
	/// </summary>
	public int? AssigneeUserId { get; set; }

	/// <summary>
	/// アサインユーザー名
	/// </summary>
	public string? AssigneeUsername { get; set; }

}
