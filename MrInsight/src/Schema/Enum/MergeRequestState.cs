namespace MrInsight;

/// <summary>
/// GitLab でのマージリクエストの状態
/// </summary>
public enum MergeRequestState
{
	/// <summary>
	/// オープン
	/// </summary>
	Opened = 1,

	/// <summary>
	/// クローズ
	/// </summary>
	Closed = 2,

	/// <summary>
	/// ロック
	/// </summary>
	Locked = 3,

	/// <summary>
	/// マージ済み
	/// </summary>
	Merged = 4,

}
