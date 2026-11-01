namespace MrInsight;

/// <summary>
/// 定義済みの値かを判定する。
/// GitLab でのマージリクエストの状態
/// </summary>
public static class MergeRequestStateValidator
{
	/// <summary>
	/// 定義済みの値かを判定する
	/// </summary>
	public static bool IsDefined(MergeRequestState value)
	{
		switch (value)
		{
			case MergeRequestState.Opened:
			case MergeRequestState.Closed:
			case MergeRequestState.Locked:
			case MergeRequestState.Merged:
				return true;

			default:
				return false;
		}
	}
}
