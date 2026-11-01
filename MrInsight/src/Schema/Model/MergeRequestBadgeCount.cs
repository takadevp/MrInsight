namespace MrInsight;

/// <summary>
/// マージリクエストの作成者とバッジごとの、コメントに付いたバッジの件数。
/// 1件のコメントに同じバッジが何度書かれても1件と数える
/// </summary>
public class MergeRequestBadgeCount
{
	/// <summary>
	/// 作成者ユーザー名
	/// </summary>
	public string? AuthorUsername { get; set; }

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
	/// その作成者に付いたバッジ全体のうち、このバッジが占める百分率
	/// </summary>
	public double? Ratio { get; set; }

}
