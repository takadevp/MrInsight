using App.Common;
using Microsoft.EntityFrameworkCore;

namespace MrInsight;

/// <summary>
/// GitLab から取り込んだマージリクエスト1件
/// </summary>
public class GMergeRequest : AbstractEntity
{
	/// <summary>
	/// このアプリの中でマージリクエストを一意に表す
	/// </summary>
	public int Id { get; init; }

	/// <summary>
	/// GitLab 全体で一意のマージリクエストのID
	/// </summary>
	public int GitlabId { get; set; }

	/// <summary>
	/// GitLab のプロジェクトのID
	/// </summary>
	public int ProjectId { get; set; }

	/// <summary>
	/// プロジェクトの中の番号。
	/// 画面で !123 と出るもの
	/// </summary>
	public int Iid { get; set; }

	/// <summary>
	/// タイトル
	/// </summary>
	public required string Title { get; set; }

	/// <summary>
	/// マージリクエストの説明欄
	/// </summary>
	public required string Description { get; set; }

	/// <summary>
	/// 状態
	/// </summary>
	public MergeRequestState State { get; set; }

	/// <summary>
	/// 下書きのままか
	/// </summary>
	public bool Draft { get; set; }

	/// <summary>
	/// 作成した人の GitLab のユーザーID
	/// </summary>
	public int AuthorId { get; set; }

	/// <summary>
	/// 作成した人の GitLab のユーザー名
	/// </summary>
	public required string AuthorUsername { get; set; }

	/// <summary>
	/// 作成した人の表示名
	/// </summary>
	public required string AuthorName { get; set; }

	/// <summary>
	/// ソースブランチ
	/// </summary>
	public required string SourceBranch { get; set; }

	/// <summary>
	/// ターゲットブランチ
	/// </summary>
	public required string TargetBranch { get; set; }

	/// <summary>
	/// 無ければ空
	/// </summary>
	public required string Milestone { get; set; }

	/// <summary>
	/// GitLab で作成された日時
	/// </summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>
	/// GitLab で最後に更新された日時
	/// </summary>
	public DateTime UpdatedAt { get; set; }

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
	public required string MergedByUsername { get; set; }

	/// <summary>
	/// 人が書いたコメントの数
	/// </summary>
	public int CommentCount { get; set; }

	/// <summary>
	/// 賛成数
	/// </summary>
	public int UpvoteCount { get; set; }

	/// <summary>
	/// 反対数
	/// </summary>
	public int DownvoteCount { get; set; }

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
	public bool HasConflicts { get; set; }

	/// <summary>
	/// GitLab の画面のURL
	/// </summary>
	public required string WebUrl { get; set; }

	/// <summary>
	/// 保存する
	/// </summary>
	public void Save()
	{
		if (_state == EntityState.Added)
		{
			DbContextScope.GlobalContext.MergeRequests.Add(this);
		}
		else
		{
			DbContextScope.GlobalContext.MergeRequests.Update(this);
		}

		DbContextScope.GlobalContext.SaveChanges();
	}

	/// <summary>
	/// 削除する
	/// </summary>
	public void Delete()
	{
		DbContextScope.GlobalContext.MergeRequests.Remove(this);
		DbContextScope.GlobalContext.SaveChanges();
	}

	/// <summary>
	/// デフォルト値で生成する
	/// </summary>
	public static GMergeRequest CreateDefault(int gitlabId, int projectId, int iid, string title, string description, MergeRequestState state, bool draft, int authorId, string authorUsername, string authorName, string sourceBranch, string targetBranch, string milestone, DateTime createdAt, DateTime updatedAt, DateTime? mergedAt, DateTime? closedAt, string mergedByUsername, int commentCount, int upvoteCount, int downvoteCount, int? additionCount, int? deletionCount, int? changedFileCount, bool hasConflicts, string webUrl)
	{
		var entity = new GMergeRequest
		{
			GitlabId = gitlabId,
			ProjectId = projectId,
			Iid = iid,
			Title = title,
			Description = description,
			State = state,
			Draft = draft,
			AuthorId = authorId,
			AuthorUsername = authorUsername,
			AuthorName = authorName,
			SourceBranch = sourceBranch,
			TargetBranch = targetBranch,
			Milestone = milestone,
			CreatedAt = createdAt,
			UpdatedAt = updatedAt,
			MergedAt = mergedAt,
			ClosedAt = closedAt,
			MergedByUsername = mergedByUsername,
			CommentCount = commentCount,
			UpvoteCount = upvoteCount,
			DownvoteCount = downvoteCount,
			AdditionCount = additionCount,
			DeletionCount = deletionCount,
			ChangedFileCount = changedFileCount,
			HasConflicts = hasConflicts,
			WebUrl = webUrl,
		};
		entity._state = EntityState.Added;
		return entity;
	}

	/// <summary>
	/// プロジェクトIDとタイトルと本文と作成者ユーザー名と開始日と終了日で取得する
	/// </summary>
	public static List<GMergeRequest> FindByProjectIdAndTitleAndDescriptionAndAuthorUsernameAndStartDateAndEndDate(int projectId, string title, string description, string authorUsername, DateTime? startDate, DateTime? endDate)
	{
		var targetGMergeRequestList = DbContextScope.GlobalContext.MergeRequests
			.Where(entity => entity.ProjectId == projectId && entity.Title.Contains(title) && entity.Description.Contains(description) && entity.AuthorUsername.Contains(authorUsername) && (startDate == null || entity.CreatedAt >= startDate) && (endDate == null || entity.CreatedAt.Date <= endDate))
			.OrderByDescending(entity => entity.CreatedAt)
			.ToList();
		return targetGMergeRequestList;
	}

	/// <summary>
	/// GitLabIDで取得する
	/// </summary>
	public static List<GMergeRequest> FindByGitlabIdList(List<int> gitlabIdList)
	{
		var targetGMergeRequestList = DbContextScope.GlobalContext.MergeRequests
			.Where(entity => gitlabIdList.Contains(entity.GitlabId))
			.OrderByDescending(entity => entity.CreatedAt)
			.ToList();
		return targetGMergeRequestList;
	}

	/// <summary>
	/// プロジェクトIDで絞り、並べ替えて先頭の1件を取得する
	/// </summary>
	public static GMergeRequest? FindLatestByProjectId(int projectId)
	{
		var targetGMergeRequest = DbContextScope.GlobalContext.MergeRequests
			.Where(entity => entity.ProjectId == projectId)
			.OrderByDescending(entity => entity.UpdatedAt)
			.FirstOrDefault();
		return targetGMergeRequest;
	}

	/// <summary>
	/// プロジェクトIDで集計する
	/// </summary>
	public static List<MergeRequestAuthorCount> SummarizeMergeRequestAuthorCountByProjectId(int projectId)
	{
		// 集計・一覧の概念は保存しないので、DB では絞って列を選ぶだけにし、読み込んでから MergeRequestAuthorCount に詰める
		var rowMergeRequestAuthorCountList = (
			from entity in DbContextScope.GlobalContext.MergeRequests
			where entity.ProjectId == projectId
			select new
			{
				AuthorUsername = entity.AuthorUsername,
			})
			.ToList();
		var targetMergeRequestAuthorCountList = rowMergeRequestAuthorCountList
			.GroupBy(row => new
			{
				AuthorUsername = row.AuthorUsername,
			})
			.Select(group => new MergeRequestAuthorCount
			{
				AuthorUsername = group.Key.AuthorUsername,
				Count = group.Count(),
			})
			.ToList();

		// 割合 で並べられるよう、並べ替えと件数制限は 割合 を入れた後に行う
		var sortedMergeRequestAuthorCountList = targetMergeRequestAuthorCountList
			.OrderByDescending(summary => summary.Count)
			.ThenBy(summary => summary.AuthorUsername)
			.ToList();
		return sortedMergeRequestAuthorCountList;
	}

	/// <summary>
	/// プロジェクトIDで集計する
	/// </summary>
	public static List<MergeRequestAuthorMonthlyCount> SummarizeMergeRequestAuthorMonthlyCountByProjectId(int projectId)
	{
		// 集計・一覧の概念は保存しないので、DB では絞って列を選ぶだけにし、読み込んでから MergeRequestAuthorMonthlyCount に詰める
		var rowMergeRequestAuthorMonthlyCountList = (
			from entity in DbContextScope.GlobalContext.MergeRequests
			where entity.ProjectId == projectId
			select new
			{
				CreatedAt = entity.CreatedAt,
				AuthorUsername = entity.AuthorUsername,
			})
			.ToList();
		var targetMergeRequestAuthorMonthlyCountList = rowMergeRequestAuthorMonthlyCountList
			.GroupBy(row => new
			{
				Year = row.CreatedAt.Year,
				Month = row.CreatedAt.Month,
				AuthorUsername = row.AuthorUsername,
			})
			.Select(group => new MergeRequestAuthorMonthlyCount
			{
				Year = group.Key.Year,
				Month = group.Key.Month,
				AuthorUsername = group.Key.AuthorUsername,
				Count = group.Count(),
			})
			.ToList();

		// 割合 で並べられるよう、並べ替えと件数制限は 割合 を入れた後に行う
		var sortedMergeRequestAuthorMonthlyCountList = targetMergeRequestAuthorMonthlyCountList
			.OrderByDescending(summary => summary.Year)
			.ThenByDescending(summary => summary.Month)
			.ThenByDescending(summary => summary.Count)
			.ThenBy(summary => summary.AuthorUsername)
			.ToList();
		return sortedMergeRequestAuthorMonthlyCountList;
	}

}
