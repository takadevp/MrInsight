namespace MrInsight;

/// <summary>
/// バッジごとの、コメントに付いたバッジの件数と、そのバッジを受けたマージリクエストの作成者の人数。
/// 1件のコメントに同じバッジが何度書かれても1件と数える
/// </summary>
public class MergeRequestTeamBadgeCount
{
	/// <summary>
	/// コメント本文の [design-badge] の design の部分。
	/// 登録したバッジだけを数える
	/// </summary>
	public string? BadgeName { get; set; }

	/// <summary>
	/// 件数
	/// </summary>
	public int? Count { get; set; }

	/// <summary>
	/// 付いたバッジ全体のうち、このバッジが占める百分率
	/// </summary>
	public double? Ratio { get; set; }

	/// <summary>
	/// このバッジを受けたマージリクエストの作成者の人数
	/// </summary>
	public int? ReceivedAuthorCount { get; set; }

}
