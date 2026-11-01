namespace MrInsight;

/// <summary>
/// GitLab から取り込んだマージリクエスト1件
/// </summary>
public class MergeRequest
{
	/// <summary>
	/// このアプリの中でマージリクエストを一意に表す
	/// </summary>
	public int? Id { get; set; }

	/// <summary>
	/// GitLab 全体で一意のマージリクエストのID
	/// </summary>
	public int? GitlabId { get; set; }

	/// <summary>
	/// GitLab のプロジェクトのID
	/// </summary>
	public int? ProjectId { get; set; }

	/// <summary>
	/// プロジェクトの中の番号。
	/// 画面で !123 と出るもの
	/// </summary>
	public int? Iid { get; set; }

	/// <summary>
	/// タイトル
	/// </summary>
	public string? Title { get; set; }

	/// <summary>
	/// マージリクエストの説明欄
	/// </summary>
	public string? Description { get; set; }

	/// <summary>
	/// 状態
	/// </summary>
	public MergeRequestState? State { get; set; }

	/// <summary>
	/// 下書きのままか
	/// </summary>
	public bool? Draft { get; set; }

	/// <summary>
	/// 作成した人の GitLab のユーザーID
	/// </summary>
	public int? AuthorId { get; set; }

	/// <summary>
	/// 作成した人の GitLab のユーザー名
	/// </summary>
	public string? AuthorUsername { get; set; }

	/// <summary>
	/// 作成した人の表示名
	/// </summary>
	public string? AuthorName { get; set; }

	/// <summary>
	/// ソースブランチ
	/// </summary>
	public string? SourceBranch { get; set; }

	/// <summary>
	/// ターゲットブランチ
	/// </summary>
	public string? TargetBranch { get; set; }

	/// <summary>
	/// 無ければ空
	/// </summary>
	public string? Milestone { get; set; }

	/// <summary>
	/// GitLab で作成された日時
	/// </summary>
	public DateTime? CreatedAt { get; set; }

	/// <summary>
	/// GitLab で最後に更新された日時
	/// </summary>
	public DateTime? UpdatedAt { get; set; }

	/// <summary>
	/// マージされた日時
	/// </summary>
	public DateTime? MergedAt { get; set; }

	/// <summary>
	/// クローズされた日時
	/// </summary>
	public DateTime? ClosedAt { get; set; }

	/// <summary>
	/// マージされていなければ空
	/// </summary>
	public string? MergedByUsername { get; set; }

	/// <summary>
	/// 人が書いたコメントの数
	/// </summary>
	public int? CommentCount { get; set; }

	/// <summary>
	/// 賛成数
	/// </summary>
	public int? UpvoteCount { get; set; }

	/// <summary>
	/// 反対数
	/// </summary>
	public int? DownvoteCount { get; set; }

	/// <summary>
	/// 追加された行数。
	/// 差分が取れなければ空
	/// </summary>
	public int? AdditionCount { get; set; }

	/// <summary>
	/// 削除された行数。
	/// 差分が取れなければ空
	/// </summary>
	public int? DeletionCount { get; set; }

	/// <summary>
	/// 変更されたファイルの数。
	/// 差分が取れなければ空
	/// </summary>
	public int? ChangedFileCount { get; set; }

	/// <summary>
	/// コンフリクトあり
	/// </summary>
	public bool? HasConflicts { get; set; }

	/// <summary>
	/// GitLab の画面のURL
	/// </summary>
	public string? WebUrl { get; set; }

	/// <summary>
	/// ラベル一覧
	/// </summary>
	public List<MergeRequestLabel> LabelList { get; set; } = [];

	/// <summary>
	/// アサイン一覧
	/// </summary>
	public List<MergeRequestAssignee> AssigneeList { get; set; } = [];

	/// <summary>
	/// レビュアー一覧
	/// </summary>
	public List<MergeRequestReviewer> ReviewerList { get; set; } = [];

	/// <summary>
	/// スレッド一覧
	/// </summary>
	public List<MergeRequestThread> ThreadList { get; set; } = [];

}
