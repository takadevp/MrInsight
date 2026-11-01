using App.Common;
using Microsoft.EntityFrameworkCore;

namespace MrInsight;

/// <summary>
/// マージリクエストのレビューを頼まれた人1人
/// </summary>
public class GMergeRequestReviewer : AbstractEntity
{
	/// <summary>
	/// ID
	/// </summary>
	public int Id { get; init; }

	/// <summary>
	/// 所属するマージリクエスト
	/// </summary>
	public int MergeRequestId { get; set; }

	/// <summary>
	/// GitLab のユーザーID
	/// </summary>
	public int ReviewerUserId { get; set; }

	/// <summary>
	/// レビュアーユーザー名
	/// </summary>
	public required string ReviewerUsername { get; set; }

	/// <summary>
	/// 保存する
	/// </summary>
	public void Save()
	{
		if (_state == EntityState.Added)
		{
			DbContextScope.GlobalContext.MergeRequestReviewers.Add(this);
		}
		else
		{
			DbContextScope.GlobalContext.MergeRequestReviewers.Update(this);
		}

		DbContextScope.GlobalContext.SaveChanges();
	}

	/// <summary>
	/// 削除する
	/// </summary>
	public void Delete()
	{
		DbContextScope.GlobalContext.MergeRequestReviewers.Remove(this);
		DbContextScope.GlobalContext.SaveChanges();
	}

	/// <summary>
	/// デフォルト値で生成する
	/// </summary>
	public static GMergeRequestReviewer CreateDefault(int mergeRequestId, int reviewerUserId, string reviewerUsername)
	{
		var entity = new GMergeRequestReviewer
		{
			MergeRequestId = mergeRequestId,
			ReviewerUserId = reviewerUserId,
			ReviewerUsername = reviewerUsername,
		};
		entity._state = EntityState.Added;
		return entity;
	}

	/// <summary>
	/// マージリクエストIDで取得する
	/// </summary>
	public static List<GMergeRequestReviewer> FindByMergeRequestIdList(List<int> mergeRequestIdList)
	{
		var targetGMergeRequestReviewerList = DbContextScope.GlobalContext.MergeRequestReviewers
			.Where(entity => mergeRequestIdList.Contains(entity.MergeRequestId))
			.OrderBy(entity => entity.Id)
			.ToList();
		return targetGMergeRequestReviewerList;
	}

	/// <summary>
	/// マージリクエストIDでまとめて削除する
	/// </summary>
	public static void DeleteByMergeRequestIdList(List<int> mergeRequestIdList)
	{
		var targetGMergeRequestReviewerList = DbContextScope.GlobalContext.MergeRequestReviewers
			.Where(entity => mergeRequestIdList.Contains(entity.MergeRequestId))
			.ToList();

		if (targetGMergeRequestReviewerList.Count > 0)
		{
			DeleteAll(DbContextScope.GlobalContext, targetGMergeRequestReviewerList);
		}
	}

}
