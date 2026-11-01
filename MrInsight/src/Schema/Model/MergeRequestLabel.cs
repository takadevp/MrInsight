namespace MrInsight;

/// <summary>
/// マージリクエストに付いたラベル1つ
/// </summary>
public class MergeRequestLabel
{
	/// <summary>
	/// ID
	/// </summary>
	public int? Id { get; set; }

	/// <summary>
	/// ラベル名
	/// </summary>
	public string? LabelName { get; set; }

}
