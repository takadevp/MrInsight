using Microsoft.EntityFrameworkCore;

namespace MrInsight;

/// <summary>
/// DB接続クラス
/// </summary>
public sealed class ApplicationGlobalDbContext(string connectionString) : ApplicationDbContext(connectionString)
{
	/// <summary>
	/// マージリクエスト
	/// </summary>
	public DbSet<GMergeRequest> MergeRequests { get; set; }

	/// <summary>
	/// ラベル
	/// </summary>
	public DbSet<GMergeRequestLabel> MergeRequestLabels { get; set; }

	/// <summary>
	/// アサイン
	/// </summary>
	public DbSet<GMergeRequestAssignee> MergeRequestAssignees { get; set; }

	/// <summary>
	/// レビュアー
	/// </summary>
	public DbSet<GMergeRequestReviewer> MergeRequestReviewers { get; set; }

	/// <summary>
	/// スレッド
	/// </summary>
	public DbSet<GMergeRequestThread> MergeRequestThreads { get; set; }

	/// <summary>
	/// コメント
	/// </summary>
	public DbSet<GMergeRequestComment> MergeRequestComments { get; set; }

	/// <summary>
	/// プロジェクト
	/// </summary>
	public DbSet<GProject> Projects { get; set; }

	/// <summary>
	/// バッジ
	/// </summary>
	public DbSet<GBadge> Badges { get; set; }

	/// <summary>
	/// モデル作成時の設定
	/// </summary>
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.ApplyConfiguration(new GMergeRequestConfiguration());
		modelBuilder.ApplyConfiguration(new GMergeRequestLabelConfiguration());
		modelBuilder.ApplyConfiguration(new GMergeRequestAssigneeConfiguration());
		modelBuilder.ApplyConfiguration(new GMergeRequestReviewerConfiguration());
		modelBuilder.ApplyConfiguration(new GMergeRequestThreadConfiguration());
		modelBuilder.ApplyConfiguration(new GMergeRequestCommentConfiguration());
		modelBuilder.ApplyConfiguration(new GProjectConfiguration());
		modelBuilder.ApplyConfiguration(new GBadgeConfiguration());
	}

	/// <summary>
	/// DB接続クラスを作成する
	/// </summary>
	public static ApplicationGlobalDbContext Create()
	{
		return new ApplicationGlobalDbContext(AppEnvironment.ConnectionString);
	}
}
