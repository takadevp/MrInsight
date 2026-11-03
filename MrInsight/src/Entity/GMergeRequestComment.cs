using App.Common;
using Microsoft.EntityFrameworkCore;

namespace MrInsight;

/// <summary>
/// スレッドの中のコメント1件。
/// GitLab が自動で書く記録は含めない
/// </summary>
public class GMergeRequestComment : AbstractEntity
{
	/// <summary>
	/// ID
	/// </summary>
	public int Id { get; init; }

	/// <summary>
	/// 所属するスレッド
	/// </summary>
	public int ThreadId { get; set; }

	/// <summary>
	/// GitLab のノートのID
	/// </summary>
	public int GitlabCommentId { get; set; }

	/// <summary>
	/// 作成者ユーザー名
	/// </summary>
	public required string AuthorUsername { get; set; }

	/// <summary>
	/// 作成者名
	/// </summary>
	public required string AuthorName { get; set; }

	/// <summary>
	/// コメント本文
	/// </summary>
	public required string Body { get; set; }

	/// <summary>
	/// 作成日
	/// </summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>
	/// 更新日
	/// </summary>
	public DateTime UpdatedAt { get; set; }

	/// <summary>
	/// 保存する
	/// </summary>
	public void Save()
	{
		if (_state == EntityState.Added)
		{
			DbContextScope.GlobalContext.MergeRequestComments.Add(this);
		}
		else
		{
			DbContextScope.GlobalContext.MergeRequestComments.Update(this);
		}

		DbContextScope.GlobalContext.SaveChanges();
	}

	/// <summary>
	/// 削除する
	/// </summary>
	public void Delete()
	{
		DbContextScope.GlobalContext.MergeRequestComments.Remove(this);
		DbContextScope.GlobalContext.SaveChanges();
	}

	/// <summary>
	/// デフォルト値で生成する
	/// </summary>
	public static GMergeRequestComment CreateDefault(int threadId, int gitlabCommentId, string authorUsername, string authorName, string body, DateTime createdAt, DateTime updatedAt)
	{
		var entity = new GMergeRequestComment
		{
			ThreadId = threadId,
			GitlabCommentId = gitlabCommentId,
			AuthorUsername = authorUsername,
			AuthorName = authorName,
			Body = body,
			CreatedAt = createdAt,
			UpdatedAt = updatedAt,
		};
		entity._state = EntityState.Added;
		return entity;
	}

	/// <summary>
	/// スレッドIDで取得する
	/// </summary>
	public static List<GMergeRequestComment> FindByThreadIdList(List<int> threadIdList)
	{
		var targetGMergeRequestCommentList = DbContextScope.GlobalContext.MergeRequestComments
			.Where(entity => threadIdList.Contains(entity.ThreadId))
			.OrderBy(entity => entity.CreatedAt)
			.ToList();
		return targetGMergeRequestCommentList;
	}

	/// <summary>
	/// スレッドIDで取得する
	/// </summary>
	public static List<GMergeRequestComment> FindByThreadId(int threadId)
	{
		var targetGMergeRequestCommentList = DbContextScope.GlobalContext.MergeRequestComments
			.Where(entity => entity.ThreadId == threadId)
			.OrderBy(entity => entity.CreatedAt)
			.ToList();
		return targetGMergeRequestCommentList;
	}

	/// <summary>
	/// プロジェクトIDと作成者ユーザー名で集計する
	/// </summary>
	public static List<MergeRequestBadgeCount> SummarizeMergeRequestBadgeCountByProjectIdAndAuthorUsername(int projectId, string authorUsername)
	{
		// 集計・一覧の概念は保存しないので、DB では絞って列を選ぶだけにし、読み込んでから MergeRequestBadgeCount に詰める
		// 登録先と突き合わせるので、登録された値を先に読んでおく
		var registeredBadgeNameSet = new HashSet<string>(DbContextScope.GlobalContext.Badges.Select(entity => entity.BadgeName));
		var rowMergeRequestBadgeCountList = (
			from entity in DbContextScope.GlobalContext.MergeRequestComments
			join mergeRequestThread in DbContextScope.GlobalContext.MergeRequestThreads on entity.ThreadId equals mergeRequestThread.Id
			join mergeRequest in DbContextScope.GlobalContext.MergeRequests on mergeRequestThread.MergeRequestId equals mergeRequest.Id
			where mergeRequest.ProjectId == projectId && mergeRequest.AuthorUsername == authorUsername && !DbContextScope.GlobalContext.ExcludedTitles.Any(registered => mergeRequest.Title.StartsWith(registered.TitlePrefix))
			select new
			{
				AuthorUsername = mergeRequest.AuthorUsername,
				Body = entity.Body,
			})
			.ToList();
		var targetMergeRequestBadgeCountList = rowMergeRequestBadgeCountList
			// 1行に同じ値が何度あっても1件と数える
			.SelectMany(row => System.Text.RegularExpressions.Regex.Matches(row.Body, @"\[([\w-]+)-badge]").Select(match => match.Groups[1].Value).Distinct(), (row, value) => new
			{
				row.AuthorUsername,
				row.Body,
				BadgeName = value,
			})
			.Where(row => registeredBadgeNameSet.Contains(row.BadgeName))
			.GroupBy(row => new
			{
				AuthorUsername = row.AuthorUsername,
				BadgeName = row.BadgeName,
			})
			.Select(group => new MergeRequestBadgeCount
			{
				AuthorUsername = group.Key.AuthorUsername,
				BadgeName = group.Key.BadgeName,
				Count = group.Count(),
			})
			.ToList();

		// 割合 はまとめた後の行どうしを比べるので、まとめ終えてから入れる
		var groupRatioList = targetMergeRequestBadgeCountList.GroupBy(summary => new
		{
			summary.AuthorUsername,
		});
		foreach (var ratioGroup in groupRatioList)
		{
			var totalRatio = ratioGroup.Sum(summary => summary.Count ?? 0);
			foreach (var targetSummary in ratioGroup)
			{
				targetSummary.Ratio = totalRatio > 0 ? Math.Round(100.0 * (targetSummary.Count ?? 0) / totalRatio, 1) : 0;
			}
		}

		// 割合 で並べられるよう、並べ替えと件数制限は 割合 を入れた後に行う
		var sortedMergeRequestBadgeCountList = targetMergeRequestBadgeCountList
			.OrderByDescending(summary => summary.Count)
			.ThenBy(summary => summary.BadgeName)
			.ToList();
		return sortedMergeRequestBadgeCountList;
	}

	/// <summary>
	/// プロジェクトIDで集計する
	/// </summary>
	public static List<MergeRequestTeamBadgeCount> SummarizeMergeRequestTeamBadgeCountByProjectId(int projectId)
	{
		// 集計・一覧の概念は保存しないので、DB では絞って列を選ぶだけにし、読み込んでから MergeRequestTeamBadgeCount に詰める
		// 登録先と突き合わせるので、登録された値を先に読んでおく
		var registeredBadgeNameSet = new HashSet<string>(DbContextScope.GlobalContext.Badges.Select(entity => entity.BadgeName));
		var rowMergeRequestTeamBadgeCountList = (
			from entity in DbContextScope.GlobalContext.MergeRequestComments
			join mergeRequestThread in DbContextScope.GlobalContext.MergeRequestThreads on entity.ThreadId equals mergeRequestThread.Id
			join mergeRequest in DbContextScope.GlobalContext.MergeRequests on mergeRequestThread.MergeRequestId equals mergeRequest.Id
			where mergeRequest.ProjectId == projectId && !DbContextScope.GlobalContext.ExcludedTitles.Any(registered => mergeRequest.Title.StartsWith(registered.TitlePrefix))
			select new
			{
				Body = entity.Body,
				AuthorUsername = mergeRequest.AuthorUsername,
			})
			.ToList();
		var targetMergeRequestTeamBadgeCountList = rowMergeRequestTeamBadgeCountList
			// 1行に同じ値が何度あっても1件と数える
			.SelectMany(row => System.Text.RegularExpressions.Regex.Matches(row.Body, @"\[([\w-]+)-badge]").Select(match => match.Groups[1].Value).Distinct(), (row, value) => new
			{
				row.Body,
				row.AuthorUsername,
				BadgeName = value,
			})
			.Where(row => registeredBadgeNameSet.Contains(row.BadgeName))
			.GroupBy(row => new
			{
				BadgeName = row.BadgeName,
			})
			.Select(group => new MergeRequestTeamBadgeCount
			{
				BadgeName = group.Key.BadgeName,
				Count = group.Count(),
				ReceivedAuthorCount = group.Select(row => row.AuthorUsername).Distinct().Count(),
			})
			.ToList();

		// 割合 はまとめた後の行どうしを比べるので、まとめ終えてから入れる
		var totalRatio = targetMergeRequestTeamBadgeCountList.Sum(summary => summary.Count ?? 0);
		foreach (var targetSummary in targetMergeRequestTeamBadgeCountList)
		{
			targetSummary.Ratio = totalRatio > 0 ? Math.Round(100.0 * (targetSummary.Count ?? 0) / totalRatio, 1) : 0;
		}

		// 割合 で並べられるよう、並べ替えと件数制限は 割合 を入れた後に行う
		var sortedMergeRequestTeamBadgeCountList = targetMergeRequestTeamBadgeCountList
			.OrderByDescending(summary => summary.ReceivedAuthorCount)
			.ThenByDescending(summary => summary.Count)
			.ThenBy(summary => summary.BadgeName)
			.ToList();
		return sortedMergeRequestTeamBadgeCountList;
	}

	/// <summary>
	/// プロジェクトIDと作成者ユーザー名とバッジ名で一覧にする
	/// </summary>
	public static List<MergeRequestBadgeComment> ListMergeRequestBadgeCommentByProjectIdAndAuthorUsernameAndBadgeName(int projectId, string authorUsername, string badgeName)
	{
		// 集計・一覧の概念は保存しないので、DB では絞って列を選ぶだけにし、読み込んでから MergeRequestBadgeComment に詰める
		// 登録先と突き合わせるので、登録された値を先に読んでおく
		var registeredBadgeNameSet = new HashSet<string>(DbContextScope.GlobalContext.Badges.Select(entity => entity.BadgeName));
		var rowMergeRequestBadgeCommentList = (
			from entity in DbContextScope.GlobalContext.MergeRequestComments
			join mergeRequestThread in DbContextScope.GlobalContext.MergeRequestThreads on entity.ThreadId equals mergeRequestThread.Id
			join mergeRequest in DbContextScope.GlobalContext.MergeRequests on mergeRequestThread.MergeRequestId equals mergeRequest.Id
			where mergeRequest.ProjectId == projectId && mergeRequest.AuthorUsername == authorUsername && !DbContextScope.GlobalContext.ExcludedTitles.Any(registered => mergeRequest.Title.StartsWith(registered.TitlePrefix))
			select new
			{
				CreatedAt = entity.CreatedAt,
				Iid = mergeRequest.Iid,
				Title = mergeRequest.Title,
				Body = entity.Body,
			})
			.ToList();
		var targetMergeRequestBadgeCommentList = rowMergeRequestBadgeCommentList
			// 1行に同じ値が何度あっても1件と数える
			.SelectMany(row => System.Text.RegularExpressions.Regex.Matches(row.Body, @"\[([\w-]+)-badge]").Select(match => match.Groups[1].Value).Distinct(), (row, value) => new
			{
				row.CreatedAt,
				row.Iid,
				row.Title,
				row.Body,
				BadgeName = value,
			})
			.Where(row => registeredBadgeNameSet.Contains(row.BadgeName))
			.Where(row => row.BadgeName == badgeName)
			.Select(row => new MergeRequestBadgeComment
			{
				CreatedAt = row.CreatedAt,
				Iid = row.Iid,
				Title = row.Title,
				Body = row.Body,
				BadgeName = row.BadgeName,
			})
			.ToList();

		// 割合 で並べられるよう、並べ替えと件数制限は 割合 を入れた後に行う
		var sortedMergeRequestBadgeCommentList = targetMergeRequestBadgeCommentList
			.OrderByDescending(summary => summary.CreatedAt)
			.ToList();
		return sortedMergeRequestBadgeCommentList;
	}

	/// <summary>
	/// プロジェクトIDで一覧にする
	/// </summary>
	public static List<MergeRequestCommentDetail> ListMergeRequestCommentDetailByProjectId(int projectId)
	{
		// 集計・一覧の概念は保存しないので、DB では絞って列を選ぶだけにし、読み込んでから MergeRequestCommentDetail に詰める
		var rowMergeRequestCommentDetailList = (
			from entity in DbContextScope.GlobalContext.MergeRequestComments
			join mergeRequestThread in DbContextScope.GlobalContext.MergeRequestThreads on entity.ThreadId equals mergeRequestThread.Id
			join mergeRequest in DbContextScope.GlobalContext.MergeRequests on mergeRequestThread.MergeRequestId equals mergeRequest.Id
			where mergeRequest.ProjectId == projectId && !DbContextScope.GlobalContext.ExcludedTitles.Any(registered => mergeRequest.Title.StartsWith(registered.TitlePrefix))
			select new
			{
				Iid = mergeRequest.Iid,
				Title = mergeRequest.Title,
				AuthorUsername = mergeRequest.AuthorUsername,
				FilePath = mergeRequestThread.FilePath,
				LineNumber = mergeRequestThread.LineNumber,
				Resolved = mergeRequestThread.Resolved,
				EntityAuthorUsername = entity.AuthorUsername,
				CreatedAt = entity.CreatedAt,
				Body = entity.Body,
			})
			.ToList();
		var targetMergeRequestCommentDetailList = rowMergeRequestCommentDetailList
			// 行を増やさず、抜き出した値を空白で区切って並べる。1行に同じ値が何度あっても1つにする
			.Select(row => new
			{
				row.Iid,
				row.Title,
				row.AuthorUsername,
				row.FilePath,
				row.LineNumber,
				row.Resolved,
				row.EntityAuthorUsername,
				row.CreatedAt,
				row.Body,
				BadgeName = string.Join(" ", System.Text.RegularExpressions.Regex.Matches(row.Body, @"\[([\w-]+)-badge]").Select(match => match.Groups[1].Value).Distinct()),
			})
			.Select(row => new MergeRequestCommentDetail
			{
				Iid = row.Iid,
				Title = row.Title,
				AuthorUsername = row.AuthorUsername,
				FilePath = row.FilePath,
				LineNumber = row.LineNumber,
				Resolved = row.Resolved,
				CommentAuthorUsername = row.EntityAuthorUsername,
				CreatedAt = row.CreatedAt,
				Body = row.Body,
				BadgeName = row.BadgeName,
			})
			.ToList();

		// 割合 で並べられるよう、並べ替えと件数制限は 割合 を入れた後に行う
		var sortedMergeRequestCommentDetailList = targetMergeRequestCommentDetailList
			.OrderBy(summary => summary.Iid)
			.ThenBy(summary => summary.CreatedAt)
			.ToList();
		return sortedMergeRequestCommentDetailList;
	}

	/// <summary>
	/// プロジェクトIDで集計する
	/// </summary>
	public static List<MergeRequestMonthlyBadgeCount> SummarizeMergeRequestMonthlyBadgeCountByProjectId(int projectId)
	{
		// 集計・一覧の概念は保存しないので、DB では絞って列を選ぶだけにし、読み込んでから MergeRequestMonthlyBadgeCount に詰める
		// 登録先と突き合わせるので、登録された値を先に読んでおく
		var registeredBadgeNameSet = new HashSet<string>(DbContextScope.GlobalContext.Badges.Select(entity => entity.BadgeName));
		var rowMergeRequestMonthlyBadgeCountList = (
			from entity in DbContextScope.GlobalContext.MergeRequestComments
			join mergeRequestThread in DbContextScope.GlobalContext.MergeRequestThreads on entity.ThreadId equals mergeRequestThread.Id
			join mergeRequest in DbContextScope.GlobalContext.MergeRequests on mergeRequestThread.MergeRequestId equals mergeRequest.Id
			where mergeRequest.ProjectId == projectId && !DbContextScope.GlobalContext.ExcludedTitles.Any(registered => mergeRequest.Title.StartsWith(registered.TitlePrefix))
			select new
			{
				CreatedAt = entity.CreatedAt,
				Body = entity.Body,
			})
			.ToList();
		var targetMergeRequestMonthlyBadgeCountList = rowMergeRequestMonthlyBadgeCountList
			// 1行に同じ値が何度あっても1件と数える
			.SelectMany(row => System.Text.RegularExpressions.Regex.Matches(row.Body, @"\[([\w-]+)-badge]").Select(match => match.Groups[1].Value).Distinct(), (row, value) => new
			{
				row.CreatedAt,
				row.Body,
				BadgeName = value,
			})
			.Where(row => registeredBadgeNameSet.Contains(row.BadgeName))
			.GroupBy(row => new
			{
				Year = row.CreatedAt.Year,
				Month = row.CreatedAt.Month,
				BadgeName = row.BadgeName,
			})
			.Select(group => new MergeRequestMonthlyBadgeCount
			{
				Year = group.Key.Year,
				Month = group.Key.Month,
				BadgeName = group.Key.BadgeName,
				Count = group.Count(),
			})
			.ToList();

		// 割合 はまとめた後の行どうしを比べるので、まとめ終えてから入れる
		var groupRatioList = targetMergeRequestMonthlyBadgeCountList.GroupBy(summary => new
		{
			summary.Year,
			summary.Month,
		});
		foreach (var ratioGroup in groupRatioList)
		{
			var totalRatio = ratioGroup.Sum(summary => summary.Count ?? 0);
			foreach (var targetSummary in ratioGroup)
			{
				targetSummary.Ratio = totalRatio > 0 ? Math.Round(100.0 * (targetSummary.Count ?? 0) / totalRatio, 1) : 0;
			}
		}

		// 割合 で並べられるよう、並べ替えと件数制限は 割合 を入れた後に行う
		var sortedMergeRequestMonthlyBadgeCountList = targetMergeRequestMonthlyBadgeCountList
			.OrderByDescending(summary => summary.Year)
			.ThenByDescending(summary => summary.Month)
			.ThenByDescending(summary => summary.Count)
			.ThenBy(summary => summary.BadgeName)
			.ToList();
		return sortedMergeRequestMonthlyBadgeCountList;
	}

	/// <summary>
	/// 全件を集計する
	/// </summary>
	public static List<MergeRequestUnregisteredBadgeCount> SummarizeMergeRequestUnregisteredBadgeCount()
	{
		// 集計・一覧の概念は保存しないので、DB では絞って列を選ぶだけにし、読み込んでから MergeRequestUnregisteredBadgeCount に詰める
		// 登録先と突き合わせるので、登録された値を先に読んでおく
		var registeredBadgeNameSet = new HashSet<string>(DbContextScope.GlobalContext.Badges.Select(entity => entity.BadgeName));
		var rowMergeRequestUnregisteredBadgeCountList = (
			from entity in DbContextScope.GlobalContext.MergeRequestComments
			join mergeRequestThread in DbContextScope.GlobalContext.MergeRequestThreads on entity.ThreadId equals mergeRequestThread.Id
			join mergeRequest in DbContextScope.GlobalContext.MergeRequests on mergeRequestThread.MergeRequestId equals mergeRequest.Id
			where !DbContextScope.GlobalContext.ExcludedTitles.Any(registered => mergeRequest.Title.StartsWith(registered.TitlePrefix))
			select new
			{
				Body = entity.Body,
			})
			.ToList();
		var targetMergeRequestUnregisteredBadgeCountList = rowMergeRequestUnregisteredBadgeCountList
			// 1行に同じ値が何度あっても1件と数える
			.SelectMany(row => System.Text.RegularExpressions.Regex.Matches(row.Body, @"\[([\w-]+)-badge]").Select(match => match.Groups[1].Value).Distinct(), (row, value) => new
			{
				row.Body,
				BadgeName = value,
			})
			.Where(row => !registeredBadgeNameSet.Contains(row.BadgeName))
			.GroupBy(row => new
			{
				BadgeName = row.BadgeName,
			})
			.Select(group => new MergeRequestUnregisteredBadgeCount
			{
				BadgeName = group.Key.BadgeName,
				Count = group.Count(),
			})
			.ToList();

		// 割合 で並べられるよう、並べ替えと件数制限は 割合 を入れた後に行う
		var sortedMergeRequestUnregisteredBadgeCountList = targetMergeRequestUnregisteredBadgeCountList
			.OrderByDescending(summary => summary.Count)
			.ThenBy(summary => summary.BadgeName)
			.ToList();
		return sortedMergeRequestUnregisteredBadgeCountList;
	}

	/// <summary>
	/// スレッドIDでまとめて削除する
	/// </summary>
	public static void DeleteByThreadIdList(List<int> threadIdList)
	{
		var targetGMergeRequestCommentList = DbContextScope.GlobalContext.MergeRequestComments
			.Where(entity => threadIdList.Contains(entity.ThreadId))
			.ToList();

		if (targetGMergeRequestCommentList.Count > 0)
		{
			DeleteAll(DbContextScope.GlobalContext, targetGMergeRequestCommentList);
		}
	}

}
